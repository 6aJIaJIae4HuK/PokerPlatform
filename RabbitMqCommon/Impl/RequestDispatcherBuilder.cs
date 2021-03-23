using System;
using System.Collections.Generic;

namespace RabbitMqCommon.Impl
{
    internal class RequestDispatcherBuilder
    {
        private interface IRequestHandlerRegistrator
        {
            void Register(RequestDispatcher dispatcher, ICodec codec, IPublisher publisher);
        }

        private class TypedRequestHandlerRegistrator<TRequest, TReply> : IRequestHandlerRegistrator
            where TRequest : new()
            where TReply : new()
        {
            public TypedRequestHandlerRegistrator(Func<ICodec, IPublisher, TypedRequestHandler<TRequest, TReply>> factory)
            {
                Factory = factory;
            }

            public void Register(RequestDispatcher dispatcher, ICodec codec, IPublisher publisher)
            {
                dispatcher.RegisterHandler(Factory(codec, publisher));
            }

            private readonly Func<ICodec, IPublisher, TypedRequestHandler<TRequest, TReply>> Factory;
        }

        public void AddRegistrator<TRequest, TReply>(Func<ICodec, IPublisher, TypedRequestHandler<TRequest, TReply>> factory)
            where TRequest : new()
            where TReply : new()
        {
            Registrators.Add(new TypedRequestHandlerRegistrator<TRequest, TReply>(factory));
        }

        public RequestDispatcher Build(ICodec codec, IPublisher publisher)
        {
            var dispatcher = new RequestDispatcher(codec);
            foreach (var registrator in Registrators)
            {
                registrator.Register(dispatcher, codec, publisher);
            }
            return dispatcher;
        }

        private readonly List<IRequestHandlerRegistrator> Registrators = new List<IRequestHandlerRegistrator>();
    }
}
