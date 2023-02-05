using System.Collections.Generic;
using System.Text;

namespace AzyWorks.Pooling.Pools
{
    public class StringBuilderPool : GenericPool<StringBuilder>
    {
        private Queue<StringBuilder> _queue;

        public static StringBuilderPool Instance;

        public override void Destroy()
        {
            _queue.Clear();
            _queue = null;

            Instance = null;
        }

        public override StringBuilder Get()
        {
            if (_queue.Count <= 0)
                return new StringBuilder();

            return _queue.Dequeue();
        }

        public override void Initialize()
        {
            _queue = new Queue<StringBuilder>();

            Instance = this;
        }

        public override void Push(StringBuilder obj)
        {
            obj.Clear();

            _queue.Enqueue(obj);
        }

        public override void Reset()
        {
            _queue.Clear();
        }

        public override bool TryGet(out StringBuilder result)
        {
            if (_queue.Count <= 0)
            {
                result = null;
                return false;
            }

            result = _queue.Dequeue();
            return true;
        }
    }
}
