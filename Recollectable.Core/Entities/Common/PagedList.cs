using System;
using System.Collections.Generic;
using System.Linq;

namespace Recollectable.Core.Entities.Common
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious
        {
            get => (CurrentPage > 1);
        }

        public bool HasNext
        {
            get => (CurrentPage < TotalPages);
        }

        public PagedList(List<T> items, int count, int page, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> Create(IQueryable<T> source, int page, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, page, pageSize);
        }
    }
}