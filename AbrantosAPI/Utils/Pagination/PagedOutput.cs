using System.Collections.Generic;

namespace AbrantosAPI.Utils.Pagination
{
    public class PagedOutput<T> : PagedOutputBase where T : class
    {
        public IList<T> Results { get; set; }

        public PagedOutput()
        {
            Results = new List<T>();
        }
    }
}
