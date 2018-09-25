using Recollectable.Core.Entities.Collectables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recollectable.Core.Entities.Collections
{
    public class CollectionCollectable
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("CollectionId")]
        public Collection Collection { get; set; }
        public Guid CollectionId { get; set; }

        [ForeignKey("CollectableId")]
        public Collectable Collectable { get; set; }
        public Guid CollectableId { get; set; }

        [ForeignKey("ConditionId")]
        public Condition Condition { get; set; }
        public Guid ConditionId { get; set; }
    }
}