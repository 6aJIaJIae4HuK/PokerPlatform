using RabbitMqCommon;

namespace PokerPlatformCommon
{
    public static class CodecBuildActions
    {
        public static CodecBuilder AddPokerMessages(this CodecBuilder builder)
        {
            builder = builder
                .RegisterType<Proto.Query>(1)
                .RegisterType<Proto.Answer>(2)
                .RegisterType<Proto.TimestampEvent>(3);
            return builder;
        }
    }
}
