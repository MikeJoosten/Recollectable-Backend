using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain.Models
{
    [XmlInclude(typeof(Coin))]
    [XmlInclude(typeof(Banknote))]
    public class CollectableDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public Collectable Collectable { get; set; }
        public Condition Condition { get; set; }
    }
}