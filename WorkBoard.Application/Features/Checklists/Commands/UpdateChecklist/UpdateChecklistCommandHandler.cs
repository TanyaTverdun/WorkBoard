using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklist;

public class UpdateChecklistCommandHandler
: IRequestHandler<UpdateChecklistCommand, ChecklistDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public UpdateChecklistCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<ChecklistDto> Handle(
        UpdateChecklistCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var checklist = await uow.ChecklistRepository.GetByIdAsync(
            request.ChecklistId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Checklist with ID {request.ChecklistId} was not found.");

        var card = await uow.CardRepository.GetByIdAsync(
            checklist.CardId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {checklist.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId,
            currentUserId,
            cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to modify this checklist.");
        }

        checklist.Name = request.Name;
        checklist.UpdatedAt = DateTime.UtcNow;
        checklist.UpdatedBy = currentUserId;

        try
        {
            await uow.ChecklistRepository.UpdateAsync(
                checklist, 
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return _mapper.Map<ChecklistDto>(checklist);
    }
}
