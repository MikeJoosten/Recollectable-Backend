using Recollectable.Core.Entities.Collections;
using System;
using System.Collections.Generic;

namespace Recollectable.API.Models.Users
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Collection> Collections { get; set; }
    }
}