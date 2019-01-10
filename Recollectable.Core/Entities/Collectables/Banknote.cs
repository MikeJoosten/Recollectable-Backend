namespace Recollectable.Core.Entities.Collectables
{
    public class Banknote : Currency
    {
        public double Length { get; set; }
        public double Width { get; set; }
        public string Color { get; set; }
        public string Watermark { get; set; }
        public string Signature { get; set; }
    }
}