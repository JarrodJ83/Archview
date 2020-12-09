namespace Archview.Model
{
    public record Resource
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public ResourceType ResourceType { get; init; }
        public string Domain { get; init; }

        public Resource() 
        { 
        }


    }
    public enum ResourceType
    {
        API,
        Service,
        Topic
    }
}
