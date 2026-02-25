namespace Application.Common.Exceptions;

public sealed class ForbiddenException(string message, string? errorCode = null) : AppException(message, errorCode)
{
}