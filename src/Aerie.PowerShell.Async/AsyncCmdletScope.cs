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
        private readonly BlockingCollection<Task> _queue = new BlockingCollection<Task>();

        private readonly CancellationToken _cancellationToken;

        [CanBeNull]
        private readonly SynchronizationContext _oldSynchronizationContext;

        private static readonly int _mainThreadId = Thread.CurrentThread.ManagedThreadId;

        private bool _disposed = false;

        public AsyncCmdletScope(
            [NotNull] AsyncCmdletContext context)
        {
            this._context = context;
            this._cancellationToken = context.CancellationToken;

            this._oldSynchronizationContext = SynchronizationContext.Current;
            var syncCtx = new QueueingSynchronizationContext(this._queue, this._cancellationToken);
            SynchronizationContext.SetSynchronizationContext(syncCtx);
        }

        [NotNull]
        public Task RequestAsyncOperation(
            [NotNull] Action action,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
        {
            this.CheckDisposed();

            bool executeSynchronously = false;

            if (option == AsyncOperationOption.ForceExecuteSynchronously)
            {
                executeSynchronously = true;
            }
            else if (option == AsyncOperationOption.ExecuteSynchronouslyIfPossible)
            {
                executeSynchronously = IsMainThread;
            }

            if (executeSynchronously)
            {
                action();
                return Task.CompletedTask;
            }

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
            if (this._disposed)
            {
                return;
            }

            SynchronizationContext.SetSynchronizationContext(this._oldSynchronizationContext);

            this._context.EndScope();

            this._queue.Dispose();

            this._disposed = true;
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

        private static bool IsMainThread
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId == _mainThreadId;
            }
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
