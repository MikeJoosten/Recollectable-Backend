using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class UserUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}