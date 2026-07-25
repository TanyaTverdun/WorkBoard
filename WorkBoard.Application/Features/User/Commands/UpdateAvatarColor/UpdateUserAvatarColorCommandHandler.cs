using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Features.User.Commands.UpdateAvatarColor;

namespace WorkBoard.Application.Features.User.Commands.UpdateAvatarColorж
{
    public class UpdateUserAvatarColorCommandHandler 
        : IRequestHandler<UpdateUserAvatarColorCommand>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IUserContext _userContext;

        public UpdateUserAvatarColorCommandHandler(
            IUnitOfWorkFactory unitOfWorkFactory,
            IUserContext userContext)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _userContext = userContext;
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
        }
    }
}
