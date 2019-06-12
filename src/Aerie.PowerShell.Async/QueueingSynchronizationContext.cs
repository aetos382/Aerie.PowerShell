using System;
using System.Threading;

using JetBrains.Annotations;

namespace Aerie.PowerShell.Async
{
    internal sealed class QueueingSynchronizationContext :
        SynchronizationContext
    {
        [NotNull]
        private readonly AsyncCmdletScope _scope;

        private readonly CancellationToken _cancellationToken;

        public QueueingSynchronizationContext(
            [NotNull] AsyncCmdletScope scope)
        {
            this._scope = scope;
            this._cancellationToken = scope.CancellationToken;
        }

        public override void Post(
            SendOrPostCallback callback,
            object state)
        {
            if (this._cancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.QueueOperation(callback, state, false);
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
            bool executeSynchnously)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this._scope.RequestAsyncOperation(() => callback(state), executeSynchnously, CancellationToken.None);
        }

        public override SynchronizationContext CreateCopy()
        {
            return new QueueingSynchronizationContext(this._scope);
        }
    }
}
