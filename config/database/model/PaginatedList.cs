using System.Collections.Generic;
using System.Linq;
using config.database.interfaces;

namespace config.database.model
{
    public class PaginatedList<T> : List<T>, IPaginatedList<T>
    {

        public PaginatedList(IEnumerable<T> Source, int PageIndex, int PageSize)
        {
            this.TotalCount = Source.Count();
            this.TotalPages = Source.Count() / PageSize;

            if(this.TotalCount % PageSize > 0)
            {
                TotalPages++;
            }

            this.PageSize = PageSize;
            this.PageIndex = PageIndex;
            this.AddRange(Source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }

        public PaginatedList(IList<T> Source, int PageIndex, int PageSize)
        {
            this.TotalCount = Source.Count();
            this.TotalPages = Source.Count() / PageSize;

            if(this.TotalCount % PageSize > 0)
            {
                TotalPages++;
            }

            this.PageSize = PageSize;
            this.PageIndex = PageIndex;
            this.AddRange(Source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }
        
        public PaginatedList(IQueryable<T> Source, int PageIndex, int PageSize)
        {
            this.TotalCount = Source.Count();
            this.TotalPages = Source.Count() / PageSize;

            if(this.TotalCount % PageSize > 0)
            {
                TotalPages++;
            }

            this.PageSize = PageSize;
            this.PageIndex = PageIndex;
            this.AddRange(Source.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList());
        }

        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        public bool HasPreviousPage { get { return (PageIndex > 1); }}
        public bool HasNextPage { get { return (PageIndex + 1 <= TotalPages); }}
    }
}