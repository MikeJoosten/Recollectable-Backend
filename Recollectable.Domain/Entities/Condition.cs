using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain.Entities
{
    public class Condition
    {
        public Guid Id { get; set; }
        public string Grade { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}