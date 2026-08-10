namespace AssignmentSystem.Api.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse(bool success, int statusCode, string message, T? data = default)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }
    }
}
