using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain.Entities
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<Collectable> Collectables { get; set; }
    }
}