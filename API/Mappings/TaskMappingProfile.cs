using AutoMapper;
using Contracts.Tasks;
using Domain.Entities;

namespace API.Mappings
{
    public sealed class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<TaskItem, TaskAnswers>();
        }
    }
}
