using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Models
{
    public class CollectableUpdateDto
    {
        public Guid CollectionId { get; set; }
        public Guid CollectableId { get; set; }
        public Guid ConditionId { get; set; }
    }
}