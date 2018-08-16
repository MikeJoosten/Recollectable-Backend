using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain.Entities
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