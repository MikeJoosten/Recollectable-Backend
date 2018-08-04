using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class CollectionCollectable
    {
        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }
        public Guid CollectableId { get; set; }
        public Collectable Collectable { get; set; }
        public Guid ConditionId { get; set; }
        public Condition Condition { get; set; }
    }
}