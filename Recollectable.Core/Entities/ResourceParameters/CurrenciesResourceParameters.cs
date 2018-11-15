namespace Recollectable.Core.Entities.ResourceParameters
{
    public class CurrenciesResourceParameters
    {
        private int _pageSize = 25;
        const int maxPageSize = 50;

        /// <summary>
        /// Current page number
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Shown elements per page
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        /// <summary>
        /// Type (Available types: Banknote - Coin)
        /// </summary>
        /// <example>Coin</example>
        public string Type { get; set; }

        /// <summary>
        /// Country name
        /// </summary>
        /// <example>Canada</example>
        public string Country { get; set; }

        /// <summary>
        /// Search query
        /// </summary>
        public string Search { get; set; }

        /// <summary>
        /// Orders by given term (Available terms: Id - Value - Country - ReleaseDate)
        /// </summary>
        /// <example>Country</example>
        public string OrderBy { get; set; } = "Value";

        /// <summary>
        /// Returned fields
        /// </summary>
        /// <example>Id, FaceValue, Type</example>
        public string Fields { get; set; }
    }
}