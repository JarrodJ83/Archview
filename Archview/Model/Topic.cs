namespace Archview.Model
{
    public record Topic(string Id, string Name) : Resource(Id, Name);
    public record Service(string Id, string Name) : Resource(Id, Name);
}
