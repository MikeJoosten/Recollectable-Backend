using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Collection
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid OwnerId { get; set; }
        public User Owner { get; set; }
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}