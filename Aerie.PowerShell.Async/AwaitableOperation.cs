namespace Aerie.PowerShell
{
    using System;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    internal class AwaitableOperation
    {
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

        private readonly Action _operation;

        public AwaitableOperation(
            [NotNull] Action operation)
        {
            this._operation = operation;
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
