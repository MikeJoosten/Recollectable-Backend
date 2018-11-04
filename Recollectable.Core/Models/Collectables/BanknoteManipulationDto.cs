using System;

namespace Recollectable.Core.Models.Collectables
{
    public abstract class BanknoteManipulationDto
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
        public string FrontImagePath { get; set; }
        public string BackImagePath { get; set; }
        public Guid CountryId { get; set; }
        public Guid CollectorValueId { get; set; }
    }
}