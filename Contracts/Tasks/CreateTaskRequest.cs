using System.ComponentModel.DataAnnotations;

namespace Contracts.Tasks;

public sealed class CreateTaskRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Title { get; init; } = "";
}
