using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.DTOs.Collectables
{
    public abstract class CoinManipulationDto
    {
        [Required(ErrorMessage = "FaceValue is a required field")]
        public int FaceValue { get; set; }

        [Required(ErrorMessage = "Type is a required field")]
        [MaxLength(100, ErrorMessage = "Type shouldn't contain more than 100 characters")]
        public string Type { get; set; }

        [Required(ErrorMessage = "ReleaseDate is a required field")]
        [MaxLength(100, ErrorMessage = "ReleaseDate shouldn't contain more than 100 characters")]
        public string ReleaseDate { get; set; }
        public int Mintage { get; set; }

        [MaxLength(25, ErrorMessage = "Weight shouldn't contain more than 25 characters")]
        public string Weight { get; set; }

        [MaxLength(25, ErrorMessage = "Size shouldn't contain more than 25 characters")]
        public string Size { get; set; }

        [MaxLength(50, ErrorMessage = "Metal shouldn't contain more than 25 characters")]
        public string Metal { get; set; }

        [MaxLength(250, ErrorMessage = "Note shouldn't contain more than 250 characters")]
        public string Note { get; set; }

        [MaxLength(250, ErrorMessage = "Subject shouldn't contain more than 250 characters")]
        public string Subject { get; set; }

        [MaxLength(250, ErrorMessage = "ObverseDescription shouldn't contain more than 250 characters")]
        public string ObverseDescription { get; set; }

        [MaxLength(100, ErrorMessage = "ObverseInscription shouldn't contain more than 100 characters")]
        public string ObverseInscription { get; set; }

        [MaxLength(100, ErrorMessage = "ObverseLegend shouldn't contain more than 100 characters")]
        public string ObverseLegend { get; set; }

        [MaxLength(250, ErrorMessage = "ReverseDescription shouldn't contain more than 250 characters")]
        public string ReverseDescription { get; set; }

        [MaxLength(100, ErrorMessage = "ReverseInscription shouldn't contain more than 100 characters")]
        public string ReverseInscription { get; set; }

        [MaxLength(100, ErrorMessage = "ReverseLegend shouldn't contain more than 100 characters")]
        public string ReverseLegend { get; set; }

        [MaxLength(50, ErrorMessage = "EdgeType shouldn't contain more than 50 characters")]
        public string EdgeType { get; set; }

        [MaxLength(100, ErrorMessage = "EdgeLegend shouldn't contain more than 100 characters")]
        public string EdgeLegend { get; set; }

        [MaxLength(250, ErrorMessage = "Designer shouldn't contain more than 250 characters")]
        public string Designer { get; set; }

        [MaxLength(250, ErrorMessage = "HeadOfState shouldn't contain more than 250 characters")]
        public string HeadOfState { get; set; }

        [MaxLength(50, ErrorMessage = "MintMark shouldn't contain more than 50 characters")]
        public string MintMark { get; set; }

        [MaxLength(250, ErrorMessage = "FrontImagePath shouldn't contain more than 250 characters")]
        public string FrontImagePath { get; set; }

        [MaxLength(250, ErrorMessage = "BackImagePath shouldn't contain more than 250 characters")]
        public string BackImagePath { get; set; }

        public Guid CountryId { get; set; }

        public Guid CollectorValueId { get; set; }
    }
}