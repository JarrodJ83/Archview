namespace Archview.Model
{
    public record Dependency(string ResourceId, DependencyType DependencyType, CommunicationStyle CommunicationStyle);
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
