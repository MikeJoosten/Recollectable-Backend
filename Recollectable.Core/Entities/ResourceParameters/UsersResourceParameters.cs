namespace Recollectable.Core.Entities.ResourceParameters
{
    public class UsersResourceParameters
    {
        private int _pageSize = 10;
        const int maxPageSize = 25;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string Search { get; set; }
        public string OrderBy { get; set; } = "Name";
        public string Fields { get; set; }
    }
}