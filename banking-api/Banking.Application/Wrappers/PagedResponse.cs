namespace Banking.Application.Wrappers
{
    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; }

        public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            Data = data;
            // Fórmula matemática para calcular total de páginas
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
        }
    }
}
