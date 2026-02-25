namespace Application.Common.Exceptions;

public abstract class AppException(string message, string? errorCode = null) : Exception(message)
{
    public string? ErrorCode { get; } = errorCode;
}