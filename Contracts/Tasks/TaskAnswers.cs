using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.Tasks
{
    public sealed record TaskAnswers(Guid Id, string Title, bool IsDone, DateTime CreatedAtUtc);

}
