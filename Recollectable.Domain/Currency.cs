using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Currency : Collectable
    {
        public int FaceValue { get; set; }
        public string Name { get; set; }
        public int ReleaseDate { get; set; }
        public Guid CollectorValueId { get; set; }
        public CollectorValue CollectorValue { get; set; }
        public string Size { get; set; }
        public string Condition { get; set; }
        public string ObverseDescription { get; set; }
        public string ReverseDescription { get; set; }
        public string FrontImagePath { get; set; }
        public string BackImagePath { get; set; }
    }
}