namespace Recollectable.Core.Entities.ResourceParameters
{
    public class CollectablesResourceParameters
    {
        private int _pageSize = 25;
        const int maxPageSize = 50;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string Type { get; set; }
        public string Country { get; set; }
        public string Search { get; set; }
        public string OrderBy { get; set; } = "Country";
        public string Fields { get; set; }
    }
}