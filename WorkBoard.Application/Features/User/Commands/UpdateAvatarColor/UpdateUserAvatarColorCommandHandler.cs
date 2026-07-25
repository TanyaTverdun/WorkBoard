using MediatR;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Application.Features.User.Commands.UpdateAvatarColor;

namespace WorkBoard.Application.Features.User.Commands.UpdateAvatarColorж
{
    public class UpdateUserAvatarColorCommandHandler 
        : IRequestHandler<UpdateUserAvatarColorCommand>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUserContext _userContext;
        private readonly IBoardNotificationService _notificationService;

        public UpdateUserAvatarColorCommandHandler(
            IUnitOfWorkFactory unitOfWorkFactory,
            IUserContext userContext,
            IBoardNotificationService notificationService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _userContext = userContext;
            _notificationService = notificationService;
        }

        public async Task Handle(
            UpdateUserAvatarColorCommand request,
            CancellationToken cancellationToken)
        {
            var currentUserId = _userContext.UserId
                ?? throw new UnauthorizedAccessException(
                    "User is not authenticated.");

            using var uow = _unitOfWorkFactory.Create();

            try
            {
                var affectedRows = await uow.UserRepository.UpdateAvatarColorAsync(
                    currentUserId,
                    request.AvatarColor,
                    cancellationToken);

                if (affectedRows == 0)
                {
                    throw new NotFoundException(
                        $"User with ID {currentUserId} was not found.");
                }

                uow.Commit();
            }
            catch
            {
                uow.Rollback();
                throw;
            }

            var userBoardIds = await uow.BoardMemberRepository.GetBoardIdsByUserIdAsync(
                    currentUserId,
                    cancellationToken);

            var notificationData = new UserAvatarColorUpdatedDto
            {
                UserId = currentUserId,
                AvatarColor = request.AvatarColor
            };

            var notificationTasks = userBoardIds.Select(boardId =>
                _notificationService.SendUserAvatarColorUpdatedAsync(
                    boardId,
                    notificationData,
                    cancellationToken));

            await Task.WhenAll(notificationTasks);
        }
    }
}
