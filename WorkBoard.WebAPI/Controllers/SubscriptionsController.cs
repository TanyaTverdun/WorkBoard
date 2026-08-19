using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Subscriptions;
using WorkBoard.Application.Features.Subscriptions.Commands.CancelCurrentSubscription;
using WorkBoard.Application.Features.Subscriptions.Commands.CreateCheckoutSession;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a Stripe checkout session for subscribing to the Pro Plan
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// The URL to the Stripe hosted checkout page
    /// </returns>
    /// <response code="200">
    /// Success. Returns the URL of the created Stripe checkout session
    /// </response>
    /// <response code="401">
    /// Unauthorized. If the user is not authenticated via Azure Entra ID
    /// </response>
    /// <response code="500">
    /// Internal Server Error. If a communication error with Stripe occurs
    /// </response>
    [HttpPost("create-checkout-session")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CheckoutSessionResponseDto>> CreateCheckoutSession(
        CancellationToken cancellationToken)
    {
        var command = new CreateCheckoutSessionCommand();

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancels the current user's Pro subscription
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <response code="200">
    /// Success. The subscription was successfully cancelled
    /// </response>
    /// <response code="401">
    /// Unauthorized. If the user is not authenticated via Azure Entra ID
    /// </response>
    /// <response code="500">
    /// Internal Server Error. If a communication error with Stripe occurs
    /// </response>
    [HttpDelete("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelSubscription(
        CancellationToken cancellationToken)
    {
        var command = new CancelCurrentSubscriptionCommand();

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}
