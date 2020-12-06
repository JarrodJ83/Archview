using Archview.Model;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Archview.Api.Endpoints.Registration
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IDriver _driver;

        public RegistrationController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost]
        public async Task<ActionResult> Register(Resource service)
        {
            var statementText = new StringBuilder();
            statementText.Append("CREATE (r:Resource { id:$id, name: $name})");
            var statementParameters = new Dictionary<string, object>
            {
                {"id", service.Id },
                {"name", service.Name }
            };

            var session = _driver.AsyncSession();
            var result = await session.WriteTransactionAsync(tx => tx.RunAsync(statementText.ToString(), statementParameters));
            return StatusCode(201, $"Node {result}");
        }
    }
}
