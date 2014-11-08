namespace Mcs.Models
{
    public class Page
    {
        public long Current { get; set; }

        public long TotalPages { get; set; }

        public long Size { get; set; }

        public long Skip { get; set; }
    }
}