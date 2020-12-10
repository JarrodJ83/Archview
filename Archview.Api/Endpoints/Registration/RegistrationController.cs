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
        private readonly NodeCreator _nodeCreator;

        public RegistrationController(IGraphClientFactory graphFactory)
        {
            _graphFactory = graphFactory;
            _nodeCreator = new NodeCreator(graphFactory);
        } 

        [HttpPost]
        public async Task<ActionResult> Register(ServiceRegistration registration)
        {
            try
            {
                var graph = await _graphFactory.CreateAsync();

                await _nodeCreator.CreateOrUpdate(registration.Service);

                foreach (var resource in registration.Dependencies)
                {
                    var qry = graph.Cypher
                        .Match("(svc)", "(dependency)")
                        .Where((Resource svc) => svc.Id == registration.Service.Id)
                        .AndWhere((Resource dependency) => dependency.Id == resource.Id)
                        .Merge(@"(svc)-[:DEPENDS_ON 
                                { 
                                    DependencyType: $dep.DependencyType, 
                                    CommunicationStyle: $dep.CommunicationStyle 
                                }]->(dependency)").WithParam("dep", resource);

                    await qry.ExecuteWithoutResultsAsync();
                }

                foreach (var topic in registration.PublishesToTopics)
                {
                    await _nodeCreator.CreateOrUpdate(topic);

                    var qry = graph.Cypher
                        .Match("(svc:Service)")
                        .Where((Resource svc) => svc.Id == registration.Service.Id)
                        .Merge("(t:Topic {Id: $id})")
                        .Merge(@"(svc)-[:PUBLISHES_TO]->(t)")                     
                        .WithParams(new
                        {
                            id = topic.Id
                        });

                    await qry.ExecuteWithoutResultsAsync();
                }

                foreach (var topic in registration.ConsumesFromTopics)
                {
                    await _nodeCreator.CreateOrUpdate(topic);

                    var qry = graph.Cypher
                        .Match("(svc:Service)")
                        .Where((Resource svc) => svc.Id == registration.Service.Id)
                        .Merge("(t:Topic {Id: $id})")
                        .Merge(@"(svc)-[:CONSUMES_FROM]->(t)")
                        .WithParams(new
                        {
                            id = topic.Id
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
