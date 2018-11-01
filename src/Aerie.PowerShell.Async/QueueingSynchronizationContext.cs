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
        private readonly BlockingCollection<Task> _queue = new BlockingCollection<Task>();

        private readonly CancellationToken _cancellationToken;

        private readonly int _mainThreadId;

        private bool _disposed = false;

        public QueueingSynchronizationContext(
            CancellationToken cancellationToken)
        {
            this._cancellationToken = cancellationToken;
            this._mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public override void Post(
            SendOrPostCallback callback,
            object state)
        {
            if (this._cancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.QueueOperation(callback, state, this.IsMainThread);
        }

        public override void Send(
            SendOrPostCallback callback,
            object state)
        {
            this.CheckDisposed();

            this.QueueOperation(callback, state, true);
        }

        public Task PostAsyncOperation(
            [NotNull] Action action,
            CancellationToken cancellationToken)
        {
            this.CheckDisposed();

            var task = new Task(action, cancellationToken);
            this._queue.Add(task);

            return task;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Task> GetQueuedTasks()
        {
            this.CheckDisposed();

            return this._queue.GetConsumingEnumerable(this._cancellationToken);
        }

        public void CloseQueue()
        {
            this.CheckDisposed();

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
                var task = new Task(() => callback(state), this._cancellationToken);
                this._queue.Add(task);

                if (synchronously)
                {
                    task.Wait();
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
            this.CheckDisposed();

            return new QueueingSynchronizationContext(this._cancellationToken);
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

        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(nameof(QueueingSynchronizationContext));
            }
        }
    }
}
