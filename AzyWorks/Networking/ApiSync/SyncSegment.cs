using AzyWorks.Networking.Messages;

using System;

namespace AzyWorks.Networking.ApiSync
{
    public class SyncSegment<TMessage> : SyncSegmentBase 
                        where TMessage : INetworkMessage 
    {
        public SyncSegment(Func<TMessage> clientMessage)
        {
            ClientCreateMessage = clientMessage;
        }

        public Func<TMessage> ClientCreateMessage;

        public override INetworkMessage DoSend()
        {
            if (ClientCreateMessage != null)
                return ClientCreateMessage.Invoke();
            else
                return base.DoSend();
        }
    }
}