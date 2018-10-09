using Recollectable.Core.Shared.Models;
using System.Collections.Generic;

namespace Recollectable.Core.Shared.Entities
{
    public class LinkedCollectionResource
    {
        public IEnumerable<IDictionary<string, object>> Value { get; set; }
        public IEnumerable<LinkDto> Links { get; set; }
    }
}