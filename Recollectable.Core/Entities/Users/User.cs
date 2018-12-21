using Microsoft.AspNetCore.Identity;
using Recollectable.Core.Entities.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Entities.Users
{
    public class User : IdentityUser<Guid>
    {
        [Key]
        public override Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Collection> Collections { get; set; }
    }
}