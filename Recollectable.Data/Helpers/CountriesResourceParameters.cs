using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Helpers
{
    public class CountriesResourceParameters
    {
        public string Name { get; set; }
        public string Search { get; set; }
        public string OrderBy { get; set; } = "Name";
    }
}