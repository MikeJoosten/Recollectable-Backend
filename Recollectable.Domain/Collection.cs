using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Collection
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}