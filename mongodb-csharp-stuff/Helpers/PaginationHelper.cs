namespace Mcs.Helpers
{
    using Mcs.Models;

    public static class PaginationHelper
    {
        public static Page Calculate(this Page page, long count)
        {
            if (count < 1)
            {
                page.Current = 0;
                page.TotalPages = 0;
            }

            var remainder = count % page.Size;

            page.TotalPages = (count / page.Size) + (remainder > 0 ? 1 : 0);

            page.Current = page.Current < 1 ? 1 : page.Current;

            if (page.TotalPages < page.Current)
            {
                page.Current = page.TotalPages;
            }

            if (page.Current > 0)
            {
                page.Skip = (page.Current - 1) * page.Size;
            }

            return page;
        }
    }
}