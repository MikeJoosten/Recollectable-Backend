using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain
{
    public class Collection
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid UserId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public User User { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}