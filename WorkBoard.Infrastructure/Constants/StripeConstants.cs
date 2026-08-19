namespace WorkBoard.Infrastructure.Constants;

public static class StripeConstants
{
    public const string SubscriptionMode = "subscription";

    public const string CheckoutSessionIdPlaceholder = "{CHECKOUT_SESSION_ID}";

    public static class ClientRoutes
    {
        public const string SubscriptionSuccess = $"/subscriptions?session_id={CheckoutSessionIdPlaceholder}";
        public const string SubscriptionCancel = "/subscriptions";
    }
}
