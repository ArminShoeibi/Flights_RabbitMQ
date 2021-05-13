namespace Flights.Common.RabbitMQ
{
    // DO favor using an enum instead of static constants.
    // https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/enum
    public enum QueueNames
    {
        FlightsQueue = 1,
    }
}
