namespace Recollectable.Core.Entities.Collectables
{
    public class Coin : Currency
    {
        public int Mintage { get; set; }
        public double Size { get; set; }
        public double Weight { get; set; }
        public string Metal { get; set; }
        public string Note { get; set; }
        public string Subject { get; set; }
        public string ObverseInscription { get; set; }
        public string ObverseLegend { get; set; }
        public string ReverseInscription { get; set; }
        public string ReverseLegend { get; set; }
        public string EdgeType { get; set; }
        public string EdgeLegend { get; set; }
        public string MintMark { get; set; }
    }
}