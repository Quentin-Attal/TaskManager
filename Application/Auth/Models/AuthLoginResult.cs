using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Models
{

    public sealed record AuthLoginResult(
        string AccessToken,
        string RefreshTokenPlain,
        DateTime RefreshTokenExpiresAtUtc
    );

}
