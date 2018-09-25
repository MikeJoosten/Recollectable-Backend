using System.ComponentModel.DataAnnotations;

namespace Recollectable.Core.Entities.Collectables
{
    public class Banknote : Currency
    {
        [MaxLength(250)]
        public string Color { get; set; }

        [MaxLength(250)]
        public string Watermark { get; set; }

        [MaxLength(250)]
        public string Signature { get; set; }
    }
}