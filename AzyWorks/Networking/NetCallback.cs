using System;

namespace AzyWorks.Networking
{
    public class NetCallback<T> : NetCallbackBase where T : INetMessage
    {
        public override Type TargetType { get => typeof(T); }

        public Action<T> Callback { get; private set; }

        public NetCallback<T> WithCallback(Action<T> callback) 
        {
            Callback = callback;
            return this;
        }

        public NetCallback<T> WithIsTemporary(bool isTemporary)
        {
            IsTemporary = isTemporary;
            return this;
        }

        public override bool IsValid(INetMessage message)
            => message is T;

        public override void Execute(INetMessage message)
        {
            if (Callback is null)
                return;

            if (!(message is T t))
                return;

            Callback(t);
        }
    }
}