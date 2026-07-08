using AutoMapper;
using WorkBoard.Application.Common.Dtos.Comments;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        CreateMap<Comment, CommentDto>();
    }
}
