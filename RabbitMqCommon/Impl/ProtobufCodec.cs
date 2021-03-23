using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace RabbitMqCommon.Impl
{ 
    internal class ProtobufCodec : ICodec
    {
        private const int ErrorType = 0;

        public ProtobufCodec()
        {
            RegisterType<Proto.ErrorMessage>(ErrorType);
        }

        public void RegisterType<T>(int typeId)
        {
            if (TypeIds.Contains(typeId))
            {
                throw new Exception($"Type #{typeId} already registered");
            }
            if (typeof(IMessage).IsAssignableFrom(typeof(T)))
            {
                throw new Exception($"Type {typeof(T)} is not proto-type");
            }
            if (!Types.TryAdd(typeof(T), typeId))
            {
                throw new Exception($"Type {typeof(T)} already registered");
            }
            Types[typeof(T)] = typeId;
            TypeIds.Add(typeId);
        }

        public byte[] SerializeEnvelope<T>(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            int typeId = CheckedGetTypeId<T>();
            var protobuf = obj as IMessage;
            var messageBytes = protobuf.ToByteString();
            var envelope = new Proto.Envelope
            {
                TypeId = typeId,
                MessageBytes = messageBytes
            };
            return envelope.ToByteArray();
        }

        public Envelope DeserializeEnvelope(ReadOnlyMemory<byte> bytes)
        {
            Proto.Envelope envelope = Deserialize<Proto.Envelope>(bytes, checkType: false);
            return new Envelope
            {
                TypeId = envelope.TypeId,
                Bytes = envelope.MessageBytes.Memory
            };
        }

        public T Deserialize<T>(ReadOnlyMemory<byte> bytes)
            where T : new()
        {
            return Deserialize<T>(bytes, checkType: true);
        }

        private T Deserialize<T>(ReadOnlyMemory<byte> bytes, bool checkType)
            where T : new()
        {
            if (checkType)
            {
                _ = CheckedGetTypeId<T>();
            }
            T t = new T();
            (t as IMessage).MergeFrom(bytes.ToArray()); // TODO: this is copying. How to avoid copying???
            return t;
        }

        public int CheckedGetTypeId<T>()
        {
            if (TryGetType<T>(out int typeId))
            {
                return typeId;
            }
            throw new Exception($"Type {typeof(T)} is not registered");
        }

        private bool TryGetType<T>(out int typeId)
        {
            return Types.TryGetValue(typeof(T), out typeId);
        }

        public byte[] SerializeError(Error error)
        {
            return SerializeEnvelope(new Proto.ErrorMessage { ErrorCode = 0, Str = error.Str });
        }

        public bool IsErrorType(int typeId)
        {
            return typeId == ErrorType;
        }

        public Error DeserializeError(ReadOnlyMemory<byte> bytes)
        {
            var protoError = Deserialize<Proto.ErrorMessage>(bytes, checkType: false);
            return new Error { Str = protoError.Str };
        }

        private readonly Dictionary<Type, int> Types = new Dictionary<Type, int>();
        private readonly HashSet<int> TypeIds = new HashSet<int>();
    }
}
