using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Entities
{
    public class Coin : Currency
    {
        public int Mintage { get; set; }

        [MaxLength(25)]
        public string Weight { get; set; }

        [MaxLength(50)]
        public string Metal { get; set; }

        [MaxLength(250)]
        public string Note { get; set; }

        [MaxLength(250)]
        public string Subject { get; set; }

        [MaxLength(100)]
        public string ObverseInscription { get; set; }

        [MaxLength(100)]
        public string ObverseLegend { get; set; }

        [MaxLength(100)]
        public string ReverseInscription { get; set; }

        [MaxLength(100)]
        public string ReverseLegend { get; set; }

        [MaxLength(50)]
        public string EdgeType { get; set; }

        [MaxLength(100)]
        public string EdgeLegend { get; set; }

        [MaxLength(50)]
        public string MintMark { get; set; }
    }
}