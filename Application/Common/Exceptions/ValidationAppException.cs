namespace Application.Common.Exceptions;

public sealed class ValidationAppException(string message, string? errorCode = null) : AppException(message, errorCode)
{
}