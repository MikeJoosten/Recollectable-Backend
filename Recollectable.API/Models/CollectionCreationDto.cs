using Newtonsoft.Json;
using Recollectable.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Recollectable.API.Models
{
    public class CollectionCreationDto
    {
        public string Type { get; set; }
        public Guid UserId { get; set; }
    }
}