using Recollectable.Core.Entities.Collectables;
using System;
using System.Xml.Serialization;

namespace Recollectable.Core.Models.Collectables
{
    [XmlInclude(typeof(Coin))]
    [XmlInclude(typeof(Banknote))]
    public class CollectableDto
    {
        public Guid Id { get; set; }
        public Collectable Collectable { get; set; }
        public string Condition { get; set; }
        public int Count { get; set; }
    }
}