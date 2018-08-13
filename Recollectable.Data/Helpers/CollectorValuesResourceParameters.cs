using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Helpers
{
    public class CollectorValuesResourceParameters
    {
        private int _pageSize = 25;
        const int maxPageSize = 50;

        public int Page { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string OrderBy { get; set; } = "Id";
        public string Fields { get; set; }
    }
}