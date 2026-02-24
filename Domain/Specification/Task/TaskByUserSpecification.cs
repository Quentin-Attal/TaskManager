using Domain.Entities;

namespace Domain.Specification.Task
{
    public class TaskByUserSpecification : Specification<TaskItem>
    {
        public TaskByUserSpecification(Guid userId)
            : base(t => t.UserId == userId)
        {
        }
    }
}