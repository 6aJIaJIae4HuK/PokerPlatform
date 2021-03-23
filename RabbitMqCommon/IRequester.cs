namespace RabbitMqCommon
{
    public interface IRequester
    {
        TReply Call<TRequest, TReply>(TRequest request) where TReply : new();
    }
}
