namespace Archview.Model
{
    public record Dependency(string Id, DependencyType DependencyType, CommunicationStyle CommunicationStyle);

    public enum DependencyType
    {
        Required,
        Optional
    }

    public enum CommunicationStyle
    {
        Synchronous,
        Asynchronous
    }
}
