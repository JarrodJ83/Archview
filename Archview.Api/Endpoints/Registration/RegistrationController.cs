using Archview.Model;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Neo4jClient;
using System.Collections.Generic;
using System.Text;
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

                await graph.Cypher.Merge("(service:Service { Id: $id })")
                    .OnCreate()
                    .Set("service = $service")
                    .WithParams(new
                    {
                        id = registration.Service.Id,
                        service = registration.Service
                    })
                    .ExecuteWithoutResultsAsync();

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
                        .Merge(@"(svc)-[:PUBLISHES_TO]->(t:Topic {name: $topicName})")                     
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
                        .Merge(@"(svc)-[:CONSUMES_FROM]->(t:Topic{name: $topicName})")
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
