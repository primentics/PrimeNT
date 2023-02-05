using AzyWorks.Logging;

using System;

namespace AzyWorks.Utilities
{
    public abstract class DisposableObject : IDisposable
    {
        private bool _isDisposed;

        public virtual void Dispose()
        {
            ThrowIfDisposed();

            _isDisposed = true;
        }

        public void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}