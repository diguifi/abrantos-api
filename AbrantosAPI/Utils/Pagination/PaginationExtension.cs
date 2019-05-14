using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MetraeSocial.Utils.Pagination
{
    public static class PaginationExtension
    {

        public static async Task<PagedOutput<T>> GetPagedAsync<T>(this IQueryable<T> query,
                                                    int page, int pageSize = 10) where T : class
        {
            var result = new PagedOutput<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = query.Count()
            };

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

            return result;
        }
    }
}
