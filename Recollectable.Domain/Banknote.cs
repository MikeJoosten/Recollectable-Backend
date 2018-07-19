using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Domain
{
    public class Banknote : Currency
    {
        private string Color { get; set; }
        private string Watermark { get; set; }
    }
}