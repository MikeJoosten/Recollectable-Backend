using Recollectable.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class UserDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Collection> Collections { get; set; }
    }
}