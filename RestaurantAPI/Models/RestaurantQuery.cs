namespace RestaurantAPI.Models
{
    public class RestaurantQuery
    {
        public string? SearchPhrase { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? SortBy { get; set; } = "Name";
        public SortDiraction SortDiraction { get; set; }

    }
}
