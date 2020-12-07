using Archview.Model;
using System;
using System.Collections.Generic;

namespace Archview
{
    public record ServiceRegistration(Service Service, IEnumerable<Dependency> Dependencies)
    {
    }
}
