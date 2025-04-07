namespace LibraryManagement.Core.Models.Dtos
{
    public class PaginatedResponse<T> where T : class
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public IReadOnlyList<T> Data { get; set; } = new List<T>();

        public PaginatedResponse(IReadOnlyList<T> data, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
    }
}
