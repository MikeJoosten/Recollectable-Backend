using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Recollectable.Core.Entities.Collections
{
    public class Condition
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Grade { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}