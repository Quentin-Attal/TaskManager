namespace Application.Common.Exceptions;

public sealed class NotFoundException(string message, string? errorCode = null) : AppException(message, errorCode)
{
}