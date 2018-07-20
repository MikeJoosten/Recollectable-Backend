using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class CollectableCondition
    {
        public Guid ConditionId { get; set; }
        public Condition condition { get; set; }
        public Guid CollectableId { get; set; }
        public Collectable Collectable { get; set; }
        public Guid CollectionId { get; set; }
        public Collection Collection { get; set; }
    }
}