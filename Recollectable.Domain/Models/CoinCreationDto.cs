using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class CoinCreationDto : CoinManipulationDto
    {
        public Country Country { get; set; }
        public CollectorValue CollectorValue { get; set; }
    }
}