using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Collectable> Collectables { get; set; }
    }
}