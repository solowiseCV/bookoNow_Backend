namespace BookNow.Presentation.Models
{
    public class ApiResponse<T>(bool success, string message, T? data = default, List<string>? errors = null)
    {
        public bool Success { get; set; } = success;
        public string Message { get; set; } = message;
        public T Data { get; set; } = data!;
        public List<string> Errors { get; set; } = errors ?? new List<string>();
    }

    public class ApiResponse(bool success, string message, List<string>? errors = null)
    {
        public bool Success { get; set; } = success;
        public string Message { get; set; } = message;
        public List<string> Errors { get; set; } = errors ?? new List<string>();
    }
}
