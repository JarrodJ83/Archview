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
            }
            catch (System.Exception ex)
            {

                throw;
            }
            
            return Ok();
        }
    }
}
