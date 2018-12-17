namespace Recollectable.Core.Entities.ResourceParameters
{
    public class CollectionsResourceParameters
    {
        private int _pageSize = 50;
        const int maxPageSize = 100;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string Type { get; set; } = string.Empty;
        public string Search { get; set; } = string.Empty;
        public string OrderBy { get; set; } = "Type";
        public string Fields { get; set; }
    }
}