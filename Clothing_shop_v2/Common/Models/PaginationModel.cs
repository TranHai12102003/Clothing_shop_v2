namespace Clothing_shop_v2.Common.Models
{
    public class PaginationModel<T> where T : class
    {
        //public long TotalRecords { get; set; }
        //public required IEnumerable<T> Records { get; set; }
        public long TotalRecords { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public required IEnumerable<T> Records { get; set; }
    }
}
