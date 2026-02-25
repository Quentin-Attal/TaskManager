namespace Application.Common.Exceptions;

public sealed class ConflictException(string message, string? errorCode = null) : AppException(message, errorCode)
{
}