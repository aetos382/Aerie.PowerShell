using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace Aerie.PowerShell
{
    public abstract class AsyncPSCmdlet :
        PSCmdlet,
        IAsyncCmdlet
    {
        protected AsyncPSCmdlet()
        {
        }

        public virtual Task BeginProcessingAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task ProcessRecordAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task EndProcessingAsync()
        {
            return Task.CompletedTask;
        }

        public CancellationToken CancellationToken
        {
            get
            {
                return this.GetCancellationToken();
            }
        }

        protected override void BeginProcessing()
        {
            this.DoBeginProcessingAsync();
        }

        protected override void ProcessRecord()
        {
            this.DoProcessRecordAsync();
        }

        protected override void EndProcessing()
        {
            this.DoEndProcessingAsync();
        }

        protected override void StopProcessing()
        {
            this.DoStopProcessing();
        }

        public virtual void Dispose()
        {
            this.DoDispose();
        }
    }
}
