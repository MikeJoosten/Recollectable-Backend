using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Models
{
    public abstract class BanknoteManipulationDto
    {
        [Required(ErrorMessage = "FaceValue is a required field")]
        public int FaceValue { get; set; }

        [Required(ErrorMessage = "Type is a required field")]
        [MaxLength(100, ErrorMessage = "Type shouldn't contain more than 100 characters")]
        public string Type { get; set; }

        [Required(ErrorMessage = "ReleaseDate is a required field")]
        [MaxLength(100, ErrorMessage = "ReleaseDate shouldn't contain more than 100 characters")]
        public string ReleaseDate { get; set; }

        [MaxLength(25, ErrorMessage = "Size shouldn't contain more than 25 characters")]
        public string Size { get; set; }

        [MaxLength(250, ErrorMessage = "Color shouldn't contain more than 250 characters")]
        public string Color { get; set; }

        [MaxLength(100, ErrorMessage = "Watermark shouldn't contain more than 100 characters")]
        public string Watermark { get; set; }

        [MaxLength(250, ErrorMessage = "Signature shouldn't contain more than 250 characters")]
        public string Signature { get; set; }

        [MaxLength(250, ErrorMessage = "ObverseDescription shouldn't contain more than 250 characters")]
        public string ObverseDescription { get; set; }

        [MaxLength(250, ErrorMessage = "ReverseDescription shouldn't contain more than 250 characters")]
        public string ReverseDescription { get; set; }

        [MaxLength(250, ErrorMessage = "Designer shouldn't contain more than 250 characters")]
        public string Designer { get; set; }

        [MaxLength(250, ErrorMessage = "HeadOfState shouldn't contain more than 250 characters")]
        public string HeadOfState { get; set; }

        [MaxLength(250, ErrorMessage = "FrontImagePath shouldn't contain more than 250 characters")]
        public string FrontImagePath { get; set; }

        [MaxLength(250, ErrorMessage = "BackImagePath shouldn't contain more than 250 characters")]
        public string BackImagePath { get; set; }

        [MaxLength(32, ErrorMessage = "CountryId shouldn't contain more than 32 characters")]
        public Guid CountryId { get; set; }

        [MaxLength(32, ErrorMessage = "CollectorValueId shouldn't contain more than 32 characters")]
        public Guid CollectorValueId { get; set; }
    }
}