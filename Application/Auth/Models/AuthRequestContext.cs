using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Models
{
    public sealed record AuthRequestContext(
          string? IpAddress = null,
          string? UserAgent = null
      );
}
