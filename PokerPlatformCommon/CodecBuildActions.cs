using RabbitMqCommon;

namespace PokerPlatformCommon
{
    public static class CodecBuildActions
    {
        public static CodecBuilder AddPokerMessages(this CodecBuilder builder)
        {
            builder = builder
                .RegisterType<Proto.ConnectToTableRequest>(1)
                .RegisterType<Proto.ConnectToTableReply>(2)
                .RegisterType<Proto.TimestampEvent>(3);
            return builder;
        }
    }
}
