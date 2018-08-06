using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Models
{
    public class CoinDto
    {
        public Guid Id { get; set; }
        public int FaceValue { get; set; }
        public string Type { get; set; }
        public int ReleaseDate { get; set; }
        public int Mintage { get; set; }
        public string Weight { get; set; }
        public string Size { get; set; }
        public string Metal { get; set; }
        public string Note { get; set; }
        public string Subject { get; set; }
        public string ObverseDescription { get; set; }
        public string ReverseDescription { get; set; }
        public string EdgeType { get; set; }
        public string EdgeLegend { get; set; }
        public string Designer { get; set; }
        public Country Country { get; set; }
        public CollectorValue CollectorValue { get; set; }
    }
}