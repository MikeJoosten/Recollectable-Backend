using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Entities
{
    public class Currency : Collectable
    {
        [Required]
        public int FaceValue { get; set; }

        [Required]
        [MaxLength(100)]
        public string Type { get; set; }

        [MaxLength(25)]
        public string Size { get; set; }

        [MaxLength(250)]
        public string Designer { get; set; }

        [MaxLength(250)]
        public string HeadOfState { get; set; }

        [MaxLength(250)]
        public string ObverseDescription { get; set; }

        [MaxLength(250)]
        public string ReverseDescription { get; set; }

        [MaxLength(250)]
        public string FrontImagePath { get; set; }

        [MaxLength(250)]
        public string BackImagePath { get; set; }
    }
}