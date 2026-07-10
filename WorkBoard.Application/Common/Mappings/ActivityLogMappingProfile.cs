using AutoMapper;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class ActivityLogMappingProfile : Profile
{
    public ActivityLogMappingProfile()
    {
        CreateMap<ActivityLog, ActivityLogDto>();
    }
}
