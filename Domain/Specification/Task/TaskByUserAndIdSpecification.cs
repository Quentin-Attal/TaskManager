using Domain.Entities;

namespace Domain.Specification.Task
{
    public class TaskByUserAndIdSpecification : Specification<TaskItem>
    {
        public TaskByUserAndIdSpecification(Guid userId, Guid taskId)
            : base(t => t.UserId == userId && t.Id == taskId)
        {
        }
    }
}