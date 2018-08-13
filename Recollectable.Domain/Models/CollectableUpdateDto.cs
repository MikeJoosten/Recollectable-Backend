using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class CollectableUpdateDto
    {
        public Guid CollectionId { get; set; }
        public Guid CollectableId { get; set; }
        public Guid ConditionId { get; set; }
    }
}