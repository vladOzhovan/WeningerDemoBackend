namespace WeningerDemoProject.Helpers
{
    public class QueryObject
    {
        public string? Search { get; set; } = null;
        public string? CustomerNumber { get; set; } = null;
        public string? FirstName { get; set; } = null;
        public string? SecondName { get; set; } = null;
        public string? Title { get; set; } = null;
        public string? Description { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        //public int PageNumber { get; set; } = 1;
        //public int PageSize { get; set; } = 25;
    }
}
