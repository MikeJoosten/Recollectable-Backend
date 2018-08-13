using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class CollectionCreationDto
    {
        public string Type { get; set; }
        public Guid UserId { get; set; }
    }
}