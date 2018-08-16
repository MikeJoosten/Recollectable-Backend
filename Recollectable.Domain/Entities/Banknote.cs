using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Entities
{
    public class Banknote : Currency
    {
        [MaxLength(250)]
        public string Color { get; set; }

        [MaxLength(250)]
        public string Watermark { get; set; }

        [MaxLength(250)]
        public string Signature { get; set; }
    }
}