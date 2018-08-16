using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain.Entities
{
    public class Collectable
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
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