using System;

namespace RabbitMqCommon
{
    public abstract class TypedRequestHandler<TRequest, TReply> : IRequestHandler
        where TRequest : new()
        where TReply : new()
    {
        public TypedRequestHandler(ICodec codec, IPublisher publisher)
        {
            Codec = codec;
            Publisher = publisher;
        }

        public byte[] HandleRequest(ReadOnlyMemory<byte> reqBytes)
        {
            var request = Codec.Deserialize<TRequest>(reqBytes);
            var reply = DoHandle(request);
            return Codec.SerializeEnvelope(reply);
        }

        public abstract TReply DoHandle(TRequest request);

        protected readonly ICodec Codec;
        protected readonly IPublisher Publisher;
    }
}
