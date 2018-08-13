using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Entities
{
    public class CollectionCollectable
    {
        public Guid Id { get; set; }
        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }
        public Guid CollectableId { get; set; }
        public Collectable Collectable { get; set; }
        public Guid ConditionId { get; set; }
        public Condition Condition { get; set; }
    }
}