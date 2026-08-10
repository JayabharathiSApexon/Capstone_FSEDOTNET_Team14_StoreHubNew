namespace StoreHub.API.Services.Models
{
    public class ServiceResult<T>
    {
        public bool Succeeded { get; }

        public int StatusCode { get; }

        public string? Message { get; }

        public T? Data { get; }

        private ServiceResult(bool succeeded, int statusCode, string? message, T? data)
        {
            Succeeded = succeeded;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static ServiceResult<T> Success(T data, int statusCode = StatusCodes.Status200OK)
        {
            return new ServiceResult<T>(true, statusCode, null, data);
        }

        public static ServiceResult<T> Failure(int statusCode, string message)
        {
            return new ServiceResult<T>(false, statusCode, message, default);
        }
    }
}