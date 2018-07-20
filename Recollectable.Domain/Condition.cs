using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Condition
    {
        public Guid Id { get; set; }
        public string Grade { get; set; }
        public List<CollectableCondition> CollectableConditions { get; set; }
    }
}