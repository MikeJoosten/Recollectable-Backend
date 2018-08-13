using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Entities
{
    public class Banknote : Currency
    {
        public string Color { get; set; }
        public string Watermark { get; set; }
        public string Signature { get; set; }
    }
}