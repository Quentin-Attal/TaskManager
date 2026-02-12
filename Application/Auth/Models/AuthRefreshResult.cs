using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Models
{
    public sealed record AuthRefreshResult(
        string AccessToken,
        string RefreshTokenPlain,
        DateTime RefreshTokenExpiresAtUtc
    );

}
