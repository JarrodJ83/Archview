namespace Archview.Model
{
    public record Resource(string Id, string Name, ResourceType ResourceType);
    public enum ResourceType
    {
        API,
        Service,
        Topic
    }
}
