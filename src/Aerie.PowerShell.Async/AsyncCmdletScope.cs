using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal sealed class AsyncCmdletScope :
        IDisposable
    {
        [NotNull]
        private readonly AsyncCmdletContext _context;

        [NotNull]
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly CancellationToken _cancellationToken;

        [NotNull]
        private readonly BlockingCollection<Task> _queue = new BlockingCollection<Task>();

        [CanBeNull]
        private readonly SynchronizationContext _oldSynchronizationContext;

        private bool _disposed = false;

        public AsyncCmdletScope(
            [NotNull] AsyncCmdletContext context)
        {
            this._context = context;

            this._cancellationToken = this._cancellationTokenSource.Token;

            this._oldSynchronizationContext = SynchronizationContext.Current;
            var syncCtx = new QueueingSynchronizationContext(this._queue, this._cancellationToken);
            SynchronizationContext.SetSynchronizationContext(syncCtx);
        }

        [NotNull]
        public Task QueueAsyncOperation(
            [NotNull] Action action,
            CancellationToken cancellationToken)
        {
            this.CheckDisposed();

            var task = CreateTask(action, cancellationToken);
            this._queue.Add(task);

            return task;
        }

        public void CloseQueue()
        {
            this.CheckDisposed();

            this._queue.CompleteAdding();
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<Task> GetQueuedTasks()
        {
            this.CheckDisposed();

            return this._queue.GetConsumingEnumerable();
        }

        public void Dispose()
        {
            SynchronizationContext.SetSynchronizationContext(this._oldSynchronizationContext);

            this._context.EndScope();

            this._cancellationTokenSource.Dispose();

            this._queue.Dispose();
        }

        public void Cancel()
        {
            this._cancellationTokenSource.Cancel();
        }

        public CancellationToken CancellationToken
        {
            get
            {
                return this._cancellationToken;
            }
        }

        [NotNull]
        private Task CreateTask(
            [NotNull] Action action,
            CancellationToken cancellationToken)
        {
            if (!cancellationToken.CanBeCanceled || cancellationToken == this._cancellationToken)
            {
                var simpleTask = new Task(action, this._cancellationToken);
                return simpleTask;
            }

            var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
                this._cancellationToken, cancellationToken);

            var task = new Task(action, linkedSource.Token)
                .ContinueWith(t =>
                {
                    linkedSource.Dispose();
                    t.Wait();
                },
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Current);

            return task;
        }

        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(nameof(AsyncCmdletScope));
            }
        }
    }
}
