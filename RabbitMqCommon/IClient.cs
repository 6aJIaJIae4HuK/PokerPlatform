namespace RabbitMqCommon
{
    public interface IClient
    {
        IRequester Requester { get; }

        ISubscriber Subscriber { get; }
    }
}
