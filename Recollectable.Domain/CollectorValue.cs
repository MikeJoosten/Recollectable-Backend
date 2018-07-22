using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class CollectorValue
    {
        public Guid Id { get; set; }
        public double? G4Value { get; set; }
        public double? VG8Value { get; set; }
        public double? F12Value { get; set; }
        public double? VF20Value { get; set; }
        public double? XF40Value { get; set; }
        public double? AU50Value { get; set; }
        public double? MS60Value { get; set; }
        public double? MS63Value { get; set; }
        public double? PF63Value { get; set; }
        public double? PF65Value { get; set; }
        public List<Currency> Currencies { get; set; }
    }
}