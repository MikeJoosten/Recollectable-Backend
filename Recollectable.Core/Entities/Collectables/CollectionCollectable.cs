using Recollectable.Core.Entities.Collections;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recollectable.Core.Entities.Collectables
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

        [MaxLength(50)]
        public string Condition { get; set; }
    }
}