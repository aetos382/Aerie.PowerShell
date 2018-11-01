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

        [CanBeNull]
        private readonly SynchronizationContext _oldSynchronizationContext;

        public AsyncCmdletScope(
            [NotNull] AsyncCmdletContext context)
        {
            this._context = context;

            this._oldSynchronizationContext = SynchronizationContext.Current;

            var syncCtx = new QueueingSynchronizationContext(this._queue, default);

            SynchronizationContext.SetSynchronizationContext(syncCtx);
        }

        public Task QueueAsyncOperation(
            Action action,
            CancellationToken cancellationToken)
        {
            var task = new Task(action, cancellationToken);
            this._queue.Add(task);

            return task;
        }

        public void CloseQueue()
        {
            this._queue.CompleteAdding();
        }

        public IEnumerable<Task> GetQueuedTasks()
        {
            return this._queue.GetConsumingEnumerable();
        }

        public void Dispose()
        {
            SynchronizationContext.SetSynchronizationContext(this._oldSynchronizationContext);

            this._context.EndScope();

            this._queue.Dispose();
        }
    }
}
