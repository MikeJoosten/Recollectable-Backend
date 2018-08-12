using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Models
{
    public class BanknoteCreationDto
    {
        public int FaceValue { get; set; }
        public string Type { get; set; }
        public string ReleaseDate { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Watermark { get; set; }
        public string Signature { get; set; }
        public string ObverseDescription { get; set; }
        public string ReverseDescription { get; set; }
        public string Designer { get; set; }
        public string HeadOfState { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public Guid CollectorValueId { get; set; }
        public CollectorValue CollectorValue { get; set; }
    }
}