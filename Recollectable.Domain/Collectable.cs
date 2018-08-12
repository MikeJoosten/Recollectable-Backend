using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain
{
    public class Collectable
    {
        public Guid Id { get; set; }
        public string ReleaseDate { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Guid CountryId { get; set; }
        public Country Country { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Guid CollectorValueId { get; set; }
        public CollectorValue CollectorValue { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}