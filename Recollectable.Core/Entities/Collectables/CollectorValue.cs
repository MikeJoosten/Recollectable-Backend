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

        public override bool Equals(object obj)
        {
            var item = obj as CollectorValue;

            if (item == null)
            {
                return false;
            }

            return G4.Equals(item.G4) && VG8.Equals(item.VG8) &&
                F12.Equals(item.F12) && VF20.Equals(item.VF20) &&
                XF40.Equals(item.XF40) && AU50.Equals(item.AU50) &&
                MS60.Equals(item.MS60) && MS63.Equals(item.MS63) &&
                PF60.Equals(item.PF60) && PF63.Equals(item.PF63) &&
                PF65.Equals(item.PF65);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}