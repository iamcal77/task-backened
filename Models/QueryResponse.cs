namespace MyWebAPI.Models
{
    public class QueryResponse
    {
        public int Id { get; set; }  // Add this for unique identification
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
