namespace RabbitMqCommon
{
    public interface IServer
    {
        IPublisher Publisher { get; }
    }
}
