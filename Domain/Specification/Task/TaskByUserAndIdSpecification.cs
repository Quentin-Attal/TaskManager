using Domain.Entities;

namespace Domain.Specification.Task
{
    public class TaskByUserAndIdSpecification(Guid userId, Guid taskId) : Specification<TaskItem>(t => t.UserId == userId && t.Id == taskId)
    {
    }
}