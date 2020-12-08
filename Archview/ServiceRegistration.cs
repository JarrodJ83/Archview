using Archview.Model;
using System;
using System.Collections.Generic;

namespace Archview
{
    public record ServiceRegistration
    {
        public Resource Service { get; init; }
        public List<Dependency> Dependencies { get; init; } = new List<Dependency>();
        public List<Topic> PublishesToTopics { get; init; } = new List<Topic>();
        public List<Topic> ConsumesFromTopics { get; init; } = new List<Topic>();
    }
}
