namespace Recollectable.Core.Entities.ResourceParameters
{
    public class CollectionsResourceParameters
    {
        private int _pageSize = 50;
        const int maxPageSize = 100;

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
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Search query
        /// </summary>
        public string Search { get; set; } = string.Empty;

        /// <summary>
        /// Orders by given term (Available terms: Id - Type)
        /// </summary>
        /// <example>Type</example>
        public string OrderBy { get; set; } = "Type";

        /// <summary>
        /// Returned fields
        /// </summary>
        /// <example>Id, Type, UserId</example>
        public string Fields { get; set; }
    }
}