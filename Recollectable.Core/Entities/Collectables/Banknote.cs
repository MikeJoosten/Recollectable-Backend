namespace Recollectable.Core.Entities.Collectables
{
    public class Banknote : Currency
    {
        public string Color { get; set; }
        public string Watermark { get; set; }
        public string Signature { get; set; }
    }
}