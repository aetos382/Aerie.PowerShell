using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal sealed class QueueingSynchronizationContext :
        SynchronizationContext
    {
        [NotNull]
        private readonly BlockingCollection<Task> _queue;

        private readonly CancellationToken _cancellationToken;

        private readonly int _mainThreadId;

        private bool _disposed = false;

        public QueueingSynchronizationContext(
            BlockingCollection<Task> queue,
            CancellationToken cancellationToken)
        {
            this._queue = queue;
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
            if (this._cancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.QueueOperation(callback, state, true);
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
            return new QueueingSynchronizationContext(this._queue, this._cancellationToken);
        }
    }
}
