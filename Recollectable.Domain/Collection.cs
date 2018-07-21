using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Collection
    {
        public Guid Id { get; set; }
        public User Owner { get; set; }
        public List<Collectable> Collectables { get; set; }
        public List<CollectableCondition> CollectableConditions { get; set; }
    }
}