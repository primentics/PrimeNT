using AzyWorks.Networking.Messages;

using System;

namespace AzyWorks.Networking.ApiSync
{
    public interface IApiStateBase
    {
        void RegisterClientHandlers(NetworkRegistry registry);
        void RegisterServerHandlers(NetworkRegistry registry);
        void RegisterUpdateSegments(out SyncSegmentBase[] segments);

        INetworkMessage InitialState();
    }

    public interface IApiState<TMessage> : IApiStateBase 
                          where TMessage : INetworkMessage 
    {
        void ProcessState(TMessage message);
    }
}