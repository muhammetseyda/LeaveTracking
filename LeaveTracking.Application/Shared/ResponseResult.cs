namespace LeaveTracking.Application.Shared
{
    public class ResponseResult<T>
    {
        public bool Success { get; set; }
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ResponseResult<T> SuccessResult(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ResponseResult<T> Failure(string errorCode, string? message = null) =>
            new() { Success = false, ErrorCode = errorCode, Message = message };
    }
}