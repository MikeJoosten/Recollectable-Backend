using System;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Entities.Collections
{
    public class Condition
    {
        [Key]
        public Guid Id { get; set; }

        public string Grade { get; set; }

        public string LanguageCode { get; set; }
    }
}