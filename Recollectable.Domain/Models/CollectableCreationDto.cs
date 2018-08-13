using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class CollectableCreationDto
    {
        public Guid CollectableId { get; set; }
        public Guid ConditionId { get; set; }
    }
}