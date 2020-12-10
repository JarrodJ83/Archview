using Archview.Model;
using Neo4jClient;
using System.Linq;
using System.Threading.Tasks;

namespace Archview.Api
{
    public class NodeCreator
    {
        private readonly IGraphClientFactory _graphFactory;

        public NodeCreator(IGraphClientFactory graphFactory)
        {
            _graphFactory = graphFactory;
        }

        public async Task CreateOrUpdate<T>(T resource) where T : Resource
        {
            var graph = await _graphFactory.CreateAsync();

            var result = (await graph.Cypher.Match($"(r:{typeof(T).Name})")
                .Where((T r) => r.Id == resource.Id)
                .Set("r = $r")
                .WithParams(new { r = resource })
                .Return<T>("r")
                .ResultsAsync).ToList();

            if (!result.Any())
            {
                await graph.Cypher.Create($"(r:{typeof(T).Name})")
                .Set("r = $r")
                .WithParams(new { r = resource })
                .ExecuteWithoutResultsAsync();
            }
        }
    }
}
