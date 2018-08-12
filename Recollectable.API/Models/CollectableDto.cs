using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Recollectable.API.Models
{
    [XmlInclude(typeof(Coin))]
    [XmlInclude(typeof(Banknote))]
    public class CollectableDto
    {
        public Guid Id { get; set; }
        public Collectable Collectable { get; set; }
        public Condition Condition { get; set; }
    }
}