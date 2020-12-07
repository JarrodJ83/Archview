namespace Archview.Model.Relationships
{
    public record Produces(Resource resource, Topic topic);
    public record Consumes(Resource resource, Topic topic);
}
