namespace Aerie.PowerShell
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    using JetBrains.Annotations;

    internal class QueueingSynchronizationContext :
        SynchronizationContext,
        IDisposable
    {
        private readonly BlockingCollection<Action> _queue;

        private readonly int _mainThreadId;

        private bool _disposed = false;

        public QueueingSynchronizationContext()
        {
            this._queue = new BlockingCollection<Action>();
            this._mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public override void Post(
            SendOrPostCallback callback,
            object state)
        {
            this.QueueOperation(callback, state, this.IsMainThread);
        }

        public override void Send(
            SendOrPostCallback callback,
            object state)
        {
            this.QueueOperation(callback, state, true);
        }

        public IEnumerable<Action> GetQueue()
        {
            return this._queue.GetConsumingEnumerable();
        }

        public void CloseQueue()
        {
            this._queue.CompleteAdding();
        }

        private void QueueOperation(
            [NotNull] SendOrPostCallback callback,
            object state,
            bool synchronously)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (synchronously && this.IsMainThread)
            {
                callback(state);
            }
            else
            {
                var operation = new AwaitableOperation(() => callback(state));
                this._queue.Add(operation.Run);

                if (synchronously)
                {
                    operation.Task.Wait();
                }
            }
        }

        private bool IsMainThread
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId == this._mainThreadId;
            }
        }

        public override SynchronizationContext CreateCopy()
        {
            return new QueueingSynchronizationContext();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;

            if (disposing)
            {
                this._queue.Dispose();
            }
        }
    }
}
