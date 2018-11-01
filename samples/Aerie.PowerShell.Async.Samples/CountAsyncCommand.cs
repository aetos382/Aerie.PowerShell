using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

using Aerie.PowerShell;

namespace Aerie.PowerSHell.Samples
{
    [Cmdlet("Count", "Async")]
    public sealed class CountAsyncCommand :
        Cmdlet,
        IAsyncCmdlet
    {
        [Parameter]
        public int Threads { get; set; } = 3;

        [Parameter]
        public int Steps { get; set; } = 100;
        
        [Parameter]
        public double Wait { get; set; } = 0.5;

        protected override void ProcessRecord()
        {
            this.DoProcessRecordAsync();
        }

        public Task BeginProcessingAsync()
        {
            return Task.CompletedTask;
        }

        public Task ProcessRecordAsync()
        {
            var tasks = Enumerable.Range(1, this.Threads).Select(i => this.CountAsync(i));

            var allTasks = Task.WhenAll(tasks.ToArray());

            return allTasks;
        }

        public Task EndProcessingAsync()
        {
            return Task.CompletedTask;
        }

        private async Task CountAsync(int activityId)
        {
            var progressRecord = new ProgressRecord(activityId, "Couting", $"Thread {activityId}");

            for (int i = 0; i < this.Steps; ++i)
            {
                await Task.Delay(TimeSpan.FromSeconds(this.Wait));

                progressRecord.PercentComplete = (int)((double)i * 100 / this.Steps);

                await this.WriteProgressAsync(progressRecord);
            }

            progressRecord.RecordType = ProgressRecordType.Completed;

            await this.WriteProgressAsync(progressRecord);
        }

        public void Dispose()
        {
            this.DoDispose();
        }
    }
}
