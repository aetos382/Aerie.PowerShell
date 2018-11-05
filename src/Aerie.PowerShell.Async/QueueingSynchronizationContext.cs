using System;
using System.Threading;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal sealed class QueueingSynchronizationContext :
        SynchronizationContext
    {
        [NotNull]
        private readonly AsyncCmdletScope _scope;

        private readonly CancellationToken _cancellationToken;

        private bool _disposed = false;

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

            this.QueueOperation(callback, state, AsyncOperationOption.None);
        }

        public override void Send(
            SendOrPostCallback callback,
            object state)
        {
            if (this._cancellationToken.IsCancellationRequested)
            {
                return;
            }

            this.QueueOperation(callback, state, AsyncOperationOption.ForceExecuteSynchronously);
        }

        private void QueueOperation(
            [NotNull] SendOrPostCallback callback,
            [CanBeNull] object state,
            AsyncOperationOption option)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this._scope.RequestAsyncOperation(() => callback(state), option, CancellationToken.None);
        }

        public override SynchronizationContext CreateCopy()
        {
            return new QueueingSynchronizationContext(this._scope);
        }
    }
}
