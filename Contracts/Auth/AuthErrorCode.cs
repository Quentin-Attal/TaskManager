using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Auth
{
    public enum AuthErrorCode
    {
        None = 0,
        InvalidCredentials = 1,
        InvalidEmail = 2,
        PasswordTooWeak = 3,
        PasswordNotMatch = 4,
        EmailAlreadyExists = 5
    }
}
