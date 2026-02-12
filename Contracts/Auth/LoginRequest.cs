using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Auth
{
    public sealed record LoginRequest(string Email, string Password);
}
