using AzyWorks.Networking.Messages;

using System;

namespace AzyWorks.Networking.ApiSync
{
    public class SyncSegmentBase
    {
        public float CustomSyncTime;
        public DateTime LastSyncTime;

        public IApiStateBase ApiState;

        public virtual INetworkMessage DoSend() { return null; }
    }
}