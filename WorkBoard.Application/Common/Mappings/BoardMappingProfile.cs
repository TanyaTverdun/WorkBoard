using AutoMapper;
using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Mappings;

public class BoardProfile : Profile
{
    public BoardProfile()
    {
        CreateMap<Board, BoardDto>();
    }
}
