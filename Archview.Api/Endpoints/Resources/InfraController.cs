using Archview.Model;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System.Linq;
using System.Threading.Tasks;

namespace Archview.Api.Endpoints.Registration
{
    [Route("api/Resources/[controller]")]
    [ApiController]
    public class InfraController : ControllerBase
    {
        private readonly NodeCreator _nodeCreator;

        public InfraController(IGraphClientFactory graphFactory)
        {
            _nodeCreator = new NodeCreator(graphFactory); ;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Infra infra)
        {
            await _nodeCreator.CreateOrUpdate(infra);

            return Ok();
        }
    }
}
