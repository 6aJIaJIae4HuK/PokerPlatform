using System;

namespace RabbitMqCommon
{
    public struct Envelope
    {
        public int TypeId { get; set; }
        public ReadOnlyMemory<byte> Bytes { get; set; }
    }

    public struct Error
    {
        public string Str { get; set; }
    }

    public interface ICodec
    {
        byte[] SerializeEnvelope<T>(T obj);

        byte[] SerializeError(Error error);

        Envelope DeserializeEnvelope(ReadOnlyMemory<byte> bytes);

        T Deserialize<T>(ReadOnlyMemory<byte> bytes)
            where T : new();


        bool IsErrorType(int typeId);

        Error DeserializeError(ReadOnlyMemory<byte> bytes);

        int CheckedGetTypeId<T>();
    }
}
