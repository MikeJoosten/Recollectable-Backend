using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using System;
using System.Xml.Serialization;

namespace Recollectable.Core.DTOs.Collectables
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