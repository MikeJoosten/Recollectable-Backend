using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;

namespace Recollectable.Domain.Entities
{
    public class Collection
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Type { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        [ForeignKey("UserId")]
        public User User { get; set; }
        public Guid UserId { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}