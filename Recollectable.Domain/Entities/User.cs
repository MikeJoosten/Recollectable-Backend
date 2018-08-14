using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(250)]
        public string Email { get; set; }
        public List<Collection> Collections { get; set; }
    }
}