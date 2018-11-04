using Newtonsoft.Json;
using Recollectable.Core.Entities.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Recollectable.Core.Entities.Collectables
{
    public class Collectable
    {
        [Key]
        public Guid Id { get; set; }
        public string ReleaseDate { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Guid CountryId { get; set; }

        [ForeignKey("CollectorValueId")]
        public CollectorValue CollectorValue { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Guid CollectorValueId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}