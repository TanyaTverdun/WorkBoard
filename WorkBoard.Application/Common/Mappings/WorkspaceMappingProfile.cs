using AutoMapper;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Mappings;

public class WorkspaceMappingProfile : Profile
{
    public WorkspaceMappingProfile()
    {
        CreateMap<Workspace, UserWorkspaceDto>()
            .ForMember(dest => dest.UserRole, 
                opt => opt.MapFrom((src, dest, destMember, context) =>
                context.Items.ContainsKey("UserRole") ? 
                    (WorkspaceRole)context.Items["UserRole"] : default));
    }
}
