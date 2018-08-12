using System;
using System.Collections.Generic;
using System.Text;

namespace Recollectable.Data.Helpers
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
    }
}