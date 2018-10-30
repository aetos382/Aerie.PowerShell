namespace Aerie.PowerShell
{
    using System;
    using System.Management.Automation;
    using System.Threading;
    using System.Threading.Tasks;

    using JetBrains.Annotations;

    public abstract class AsyncCmdlet :
        PSCmdlet
    {
        private readonly AsyncLocal<QueueingSynchronizationContext> _syncCtx = new AsyncLocal<QueueingSynchronizationContext>();

        protected virtual Task BeginProcessingAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task ProcessRecordAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task EndProcessingAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual Task StopProcessingAsync()
        {
            return Task.CompletedTask;
        }

        protected override void BeginProcessing()
        {
            this.DoWithSynchronizationContext(this.BeginProcessingAsync);
        }

        protected override void ProcessRecord()
        {
            this.DoWithSynchronizationContext(this.ProcessRecordAsync);
        }

        protected override void EndProcessing()
        {
            this.DoWithSynchronizationContext(this.EndProcessingAsync);
        }

        protected SynchronizationContext SynchronizationContext
        {
            get
            {
                return this._syncCtx.Value;
            }
        }

        private void DoWithSynchronizationContext(
            [NotNull] Func<Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            using (var syncCtx = new QueueingSynchronizationContext())
            {
                var oldSyncCtx = SynchronizationContext.Current;

                try
                {
                    this._syncCtx.Value = syncCtx;
                    SynchronizationContext.SetSynchronizationContext(syncCtx);

                    var task = func();
                    var awaiter = task.GetAwaiter();
                    awaiter.OnCompleted(syncCtx.CloseQueue);

                    foreach (var action in syncCtx.GetQueue())
                    {
                        action();
                    }

                    awaiter.GetResult();
                }
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(oldSyncCtx);
                    this._syncCtx.Value = null;
                }
            }
        }
    }
}
