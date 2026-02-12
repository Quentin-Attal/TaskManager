using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Auth.Options
{
    public sealed class TokenHashOptions
    {
        public const string SectionName = "TokenHash";

        // Server-side secret (pepper)
        // NEVER store this in database.
        public string Pepper { get; init; } = default!;
    }
}
