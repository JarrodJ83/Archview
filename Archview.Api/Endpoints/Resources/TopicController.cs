using Archview.Model;
using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using System.Threading.Tasks;

namespace Archview.Api.Endpoints.Registration
{
    [Route("api/Resources/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly NodeCreator _nodeCreator;

        public TopicController(IGraphClientFactory graphFactory)
        {
            _nodeCreator = new NodeCreator(graphFactory); ;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Topic topic)
        {
            await _nodeCreator.CreateOrUpdate(topic);

            return Ok();
        }
    }
}
