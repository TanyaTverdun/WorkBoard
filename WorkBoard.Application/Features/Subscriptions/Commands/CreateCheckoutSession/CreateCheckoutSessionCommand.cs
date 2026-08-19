using MediatR;
using WorkBoard.Application.Common.Dtos.Subscriptions;

namespace WorkBoard.Application.Features.Subscriptions.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionCommand 
    : IRequest<CheckoutSessionResponseDto>
{
}
