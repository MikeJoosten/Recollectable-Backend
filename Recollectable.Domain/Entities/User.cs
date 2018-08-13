using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<Collection> Collections { get; set; }
    }
}