using Archview.Model;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System.Linq;
using System.Threading.Tasks;

namespace Archview.Api.Endpoints.Registration
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IGraphClientFactory _graphFactory;

        public RegistrationController(IGraphClientFactory graphFactory)
        {
            _graphFactory = graphFactory;
        }

        [HttpPost]
        public async Task<ActionResult> Register(ServiceRegistration registration)
        {
            try
            {
                var graph = await _graphFactory.CreateAsync();

                var result = (await graph.Cypher.Match("(svc:Service)")
                    .Where((Resource svc) => svc.Id == registration.Service.Id)
                    .Set("svc = $service")
                    .WithParams(new { service = registration.Service })
                    .Return<Resource>("svc")
                    .ResultsAsync).ToList();

                if(!result.Any())
                {
                    await graph.Cypher.Create("(svc:Service)")
                    .Set("svc = $service")
                    .WithParams(new { service = registration.Service })
                    .ExecuteWithoutResultsAsync();
                }

                foreach (var resource in registration.Dependencies)
                {
                    var qry = graph.Cypher
                        .Match("(svc:Service)", "(dependency:Service)")
                        .Where((Resource svc) => svc.Id == registration.Service.Id)
                        .AndWhere((Resource dependency) => dependency.Id == resource.ResourceId)
                        .Merge(@"(svc)-[:DEPENDS_ON 
                                { 
                                    DependencyType: $dep.DependencyType, 
                                    CommunicationStyle: $dep.CommunicationStyle 
                                }]->(dependency)").WithParam("dep", resource);

                    await qry.ExecuteWithoutResultsAsync();
                }

                foreach (var topic in registration.PublishesToTopics)
                {
                    var qry = graph.Cypher
                        .Match("(svc:Service)")
                        .Where((Resource svc) => svc.Id == registration.Service.Id)
                        .Merge("(t:Topic {name: $topicName})")
                        .Merge(@"(svc)-[:PUBLISHES_TO]->(t)")                     
                        .WithParams(new
                        {
                            topicName = topic.Name
                        });

                    await qry.ExecuteWithoutResultsAsync();
                }

                foreach (var topic in registration.ConsumesFromTopics)
                {
                    var qry = graph.Cypher
                        .Match("(svc:Service)")
                        .Where((Resource svc) => svc.Id == registration.Service.Id)
                        .Merge("(t:Topic {name: $topicName})")
                        .Merge(@"(svc)-[:CONSUMES_FROM]->(t)")
                        .WithParams(new
                        {
                            topicName = topic.Name
                        });

                    await qry.ExecuteWithoutResultsAsync();
                }
            }
            catch (System.Exception ex)
            {

                throw;
            }
            
            return Ok();
        }
    }
}
