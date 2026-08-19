using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using WorkBoard.Application.Features.Subscriptions.Commands.CancelUserSubscription;
using WorkBoard.Application.Features.Subscriptions.Commands.UpgradeUserSubscription;
using WorkBoard.Infrastructure.Constants;
using WorkBoard.Infrastructure.Options;

namespace WorkBoard.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly StripeOptions _stripeOptions;
    private readonly IMediator _mediator;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(
        IOptions<StripeOptions> stripeOptions,
        IMediator mediator,
        ILogger<WebhooksController> logger)
    {
        _stripeOptions = stripeOptions.Value;
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var signatureHeader = Request.Headers[StripeConstants.SignatureHeader];

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                signatureHeader,
                _stripeOptions.WebhookSecret);

            _logger.LogInformation(
                "Received Stripe event: {Type}",
                stripeEvent.Type);

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                    if (session != null && 
                        session.Mode == StripeConstants.SubscriptionMode)
                    {
                        var userId = session.ClientReferenceId;
                        var subscriptionId = session.SubscriptionId;

                        _logger.LogInformation(
                            "User {UserId} successfully subscribed. Stripe Sub ID: {SubId}",
                            userId, subscriptionId);

                        if (Guid.TryParse(userId, out var parsedUserId))
                        {
                            await _mediator.Send(new UpgradeUserSubscriptionCommand(
                                parsedUserId, 
                                subscriptionId));
                        }
                    }
                    break;

                case EventTypes.CustomerSubscriptionUpdated:
                    break;

                case EventTypes.CustomerSubscriptionDeleted:
                    var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    if (subscription != null)
                    {
                        _logger.LogInformation("" +
                            "Subscription {SubId} deleted.",
                            subscription.Id);

                        await _mediator.Send(new CancelUserSubscriptionCommand(
                            subscription.Id));
                    }
                    break;

                default:
                    _logger.LogInformation(
                        "Unhandled event type: {Type}",
                        stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(
                ex, 
                "Stripe Webhook Signature Verification Failed.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing Stripe Webhook.");
            return StatusCode(500);
        }
    }
}
