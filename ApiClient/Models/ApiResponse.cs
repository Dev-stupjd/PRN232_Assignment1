namespace ApiClient.Models
{
    public class ApiListResponse<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; } = new();
        public int Count { get; set; }
        public string? Error { get; set; }
    }
}


