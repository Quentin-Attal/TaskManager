using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Models
{
    public sealed record RefreshTokenDescriptor(
        string PlainToken,
        string TokenHash,
        DateTime ExpiresAtUtc
    );
}
