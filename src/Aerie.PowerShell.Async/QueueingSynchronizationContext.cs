using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class QueueingSynchronizationContext :
        SynchronizationContext,
        IDisposable
    {
        [NotNull]
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

        public Task PostAsyncOperation(
            [NotNull] Action action,
            CancellationToken cancellationToken)
        {
            var operation = new AwaitableOperation(action, cancellationToken);
            this._queue.Add(operation.Run);

            return operation.Task;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Action> GetQueuedActions()
        {
            return this._queue.GetConsumingEnumerable();
        }

        public void CloseQueue()
        {
            this._queue.CompleteAdding();
        }

        private void QueueOperation(
            [NotNull] SendOrPostCallback callback,
            [CanBeNull] object state,
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
                var operation = new AwaitableOperation(() => callback(state), CancellationToken.None);
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
