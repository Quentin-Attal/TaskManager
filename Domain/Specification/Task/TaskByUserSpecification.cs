using Domain.Entities;

namespace Domain.Specification.Task
{
    public class TaskByUserSpecification(Guid userId) : Specification<TaskItem>(t => t.UserId == userId)
    {
    }
}