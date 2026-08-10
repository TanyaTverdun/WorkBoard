using AutoMapper;
using WorkBoard.Application.Features.Workspace.Commands.AddWorkspaceMember;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class WorkspaceMappingProfile : Profile
{
    public WorkspaceMappingProfile()
    {
        CreateMap<AddWorkspaceMemberCommand, WorkspaceMember>()
            .ForMember(
                dest => dest.UserId,
                opt => opt.MapFrom(src => src.TargetUserId))

            .ForMember(
                dest => dest.UserRole,
                opt => opt.MapFrom(src => src.Role));
    }
}
