using Newtonsoft.Json;
using Recollectable.Core.Entities.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Recollectable.Core.Entities.Collections
{
    public class Collection
    {
        [Key]
        public Guid Id { get; set; }
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