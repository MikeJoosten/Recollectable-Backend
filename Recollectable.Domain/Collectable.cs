using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain
{
    public class Collectable
    {
        public Guid Id { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public string Description { get; set; }
    }
}