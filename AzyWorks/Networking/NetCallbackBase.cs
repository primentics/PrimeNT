using System;

namespace AzyWorks.Networking
{
    public class NetCallbackBase
    {
        public virtual Type TargetType { get; }

        public bool IsTemporary { get; set; }

        public virtual bool IsValid(INetMessage message) => false;
        public virtual void Execute(INetMessage message) { }
    }
}
