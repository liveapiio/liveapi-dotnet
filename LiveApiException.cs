namespace LiveApi;

public sealed class LiveApiException : Exception
{
    public int Status { get; }
    public string ErrorCode { get; }

    public LiveApiException(int status, string errorCode, string message) : base(message)
    {
        Status = status;
        ErrorCode = errorCode;
    }
}
