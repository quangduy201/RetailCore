namespace RetailCore.Shared.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }

    public ApiResponse(bool success, T? data, string? message)
    {
        Success = success;
        Data = data;
        Message = message;
    }
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data)
        => new ApiResponse<T>(true, data, null);

    public static ApiResponse<T> Fail<T>(string message)
        => new ApiResponse<T>(false, default, message);
}
