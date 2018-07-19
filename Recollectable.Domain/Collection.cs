using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Collection
    {
        private Guid Id { get; set; }
        private Guid OwnerId { get; set; }
        private User Owner { get; set; }
        private Guid CollectableId { get; set; }
        private List<Collectable> Collectables { get; set; }
    }
}