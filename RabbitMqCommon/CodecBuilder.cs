namespace RabbitMqCommon
{
    public class CodecBuilder
    {
        public CodecBuilder RegisterType<T>(int typeId)
        {
            Codec.RegisterType<T>(typeId);
            return this;
        }

        public ICodec Build()
        {
            return Codec;
        }

        private readonly Impl.ProtobufCodec Codec = new Impl.ProtobufCodec();
    }
}
