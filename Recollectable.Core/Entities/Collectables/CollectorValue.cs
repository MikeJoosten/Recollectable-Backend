using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Recollectable.Core.Entities.Collectables
{
    public class CollectorValue
    {
        [Key]
        public Guid Id { get; set; }
        public double? G4 { get; set; }
        public double? VG8 { get; set; }
        public double? F12 { get; set; }
        public double? VF20 { get; set; }
        public double? XF40 { get; set; }
        public double? AU50 { get; set; }
        public double? MS60 { get; set; }
        public double? MS63 { get; set; }
        public double? PF60 { get; set; }
        public double? PF63 { get; set; }
        public double? PF65 { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<Collectable> Collectables { get; set; }
    }
}