using Archview.Model;
using System;
using System.Collections.Generic;

namespace Archview
{
    public record ServiceRegistration(Resource Service, IEnumerable<Resource> Dependencies)
    {
    }
}
