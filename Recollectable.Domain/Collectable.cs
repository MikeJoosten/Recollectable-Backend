using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Recollectable.Domain
{
    public class Collectable
    {
        public Guid Id { get; set; }
        public string ReleaseDate { get; set; }
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
        public Guid CollectorValueId { get; set; }
        public CollectorValue CollectorValue { get; set; }
        public List<CollectionCollectable> CollectionCollectables { get; set; }
    }
}