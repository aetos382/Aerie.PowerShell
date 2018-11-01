using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class AwaitableOperation
    {
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

        private readonly Action _operation;

        private readonly CancellationToken _cancellationToken;

        public AwaitableOperation(
            [NotNull] Action operation,
            CancellationToken cancellationToken)
        {
            this._operation = operation;

            this._cancellationToken = cancellationToken;

            cancellationToken.Register(() => this._tcs.TrySetCanceled(cancellationToken));
        }

        public Task Task
        {
            get
            {
                return this._tcs.Task;
            }
        }

        public void Run()
        {
            try
            {
                if (this._cancellationToken.IsCancellationRequested)
                {
                    this._tcs.TrySetCanceled(this._cancellationToken);
                }

                this._operation();
                this._tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                this._tcs.SetException(ex);
            }
        }
    }
}
