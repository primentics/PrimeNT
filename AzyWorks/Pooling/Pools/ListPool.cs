using System.Collections.Generic;

namespace AzyWorks.Pooling.Pools
{
    public class ListPool<TItem> : GenericPool<List<TItem>>
    {
        private Queue<List<TItem>> _queue;

        private static ListPool<TItem> _instance;

        public static ListPool<TItem> Instance
        {
            get
            {
                if (_instance is null)
                    PoolManager.SetPool<ListPool<TItem>>(new ListPool<TItem>());

                return _instance;
            }
        }

        public override void Destroy()
        {
            _queue.Clear();
            _queue = null;

            _instance = null;
        }

        public override List<TItem> Get()
        {
            if (_queue.Count <= 0)
                return new List<TItem>();
            else
                return _queue.Dequeue();
        }

        public override void Initialize()
        {
            _queue = new Queue<List<TItem>>();

            _instance = this;
        }

        public override void Push(List<TItem> obj)
        {
            obj.Clear();

            _queue.Enqueue(obj);
        }

        public override void Reset()
        {
            _queue.Clear();
        }

        public override bool TryGet(out List<TItem> result)
        {
            if (_queue.Count <= 0)
            {
                result = null;
                return false;
            }
            else
            {
                result = new List<TItem>();
                return true;
            }
        }
    }
}
