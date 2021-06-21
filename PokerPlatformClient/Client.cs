using System;
using System.Collections.Generic;
using System.Text;
using PokerPlatformCommon;
using PokerPlatformCommon.Proto;

namespace PokerPlatformClient
{
    public class ClientWithStrategy
    {
        public ClientWithStrategy(ulong playerId, AbstractStrategy strategy)
        {
            PlayerId = playerId;
            Strategy = strategy;
            Client = new RabbitMqCommon.RabbitMqClientBuilder(
                "127.0.0.1", "rpc",
                new RabbitMqCommon.CodecBuilder().AddPokerMessages().Build()
            ).Build();
        }

        public void ConnectToTable()
        {
            var reply = Client.Requester.Call<ConnectToTableRequest, ConnectToTableReply>(
                new ConnectToTableRequest { PlayerId = PlayerId }
            );
            Strategy.InitTable(new PokerPlatformCommon.TableView(reply.TableView));
        }

        private readonly ulong PlayerId;
        private readonly RabbitMqCommon.IClient Client;
        private readonly AbstractStrategy Strategy;
    }
}
