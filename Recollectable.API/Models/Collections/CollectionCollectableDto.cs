using Recollectable.Core.Entities.Collectables;
using System;
using System.Xml.Serialization;

namespace Recollectable.API.Models.Collections
{
    [XmlInclude(typeof(Coin))]
    [XmlInclude(typeof(Banknote))]
    public class CollectionCollectableDto
    {
        public Guid Id { get; set; }
        public Collectable Collectable { get; set; }
        public Guid ConditionId { get; set; }
    }
}