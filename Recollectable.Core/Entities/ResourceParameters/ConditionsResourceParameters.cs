namespace Recollectable.Core.Entities.ResourceParameters
{
    public class ConditionsResourceParameters
    {
        private int _pageSize = 50;
        const int maxPageSize = 100;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string Grade { get; set; }
        public string Search { get; set; }
        public string OrderBy { get; set; } = "Grade";
        public string Fields { get; set; }
    }
}