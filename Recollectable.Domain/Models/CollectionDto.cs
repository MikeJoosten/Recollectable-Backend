using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain.Models
{
    public class CollectionDto : LinkedResourceBaseDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid UserId { get; set; }
    }
}