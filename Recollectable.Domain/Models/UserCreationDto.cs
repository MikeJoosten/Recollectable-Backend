using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class UserCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Collection> Collections { get; set; } = new List<Collection>();
    }
}