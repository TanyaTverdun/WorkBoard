using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Boards.Queries.SearchAssignableUsers;

public class SearchAssignableUsersQueryHandler
    : IRequestHandler<SearchAssignableUsersQuery, IReadOnlyList<UserSearchDto>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public SearchAssignableUsersQueryHandler(
        IBoardRepository boardRepository,
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _boardRepository = boardRepository;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<UserSearchDto>> Handle(
    SearchAssignableUsersQuery request,
    CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        var board = await _boardRepository.GetByIdAsync(
            request.BoardId, 
            cancellationToken);

        if (board == null)
        {
            throw new NotFoundException(
                $"Board with ID {request.BoardId} was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return new List<UserSearchDto>().AsReadOnly();
        }

        var usersFromDb = await _userRepository.SearchAssignableUsersAsync(
            request.BoardId,
            request.SearchTerm,
            cancellationToken);

        foreach (var user in usersFromDb)
        {
            user.Initials = InitialGenerator.Generate(user.FullName);
        }

        return usersFromDb;
    }
}
