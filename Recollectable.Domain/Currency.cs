using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Currency : Collectable
    {
        public int FaceValue { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Designer { get; set; }
        public string HeadOfState { get; set; }
        public string ObverseDescription { get; set; }
        public string ReverseDescription { get; set; }
        public string FrontImagePath { get; set; }
        public string BackImagePath { get; set; }
    }
}