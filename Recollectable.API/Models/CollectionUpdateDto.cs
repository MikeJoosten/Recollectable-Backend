using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recollectable.API.Models
{
    public class CollectionUpdateDto
    {
        public string Type { get; set; }
        public Guid UserId { get; set; }
    }
}