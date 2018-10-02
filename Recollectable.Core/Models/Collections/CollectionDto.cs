using System;

namespace Recollectable.Core.Models.Collections
{
    public class CollectionDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public Guid UserId { get; set; }
    }
}