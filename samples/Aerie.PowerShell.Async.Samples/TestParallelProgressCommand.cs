using System;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

using Aerie.PowerShell.Async;

namespace Aerie.PowerShell.Samples
{
    [Cmdlet("Test", "ParallelProgress")]
    public sealed class TestParallelProgressCommand :
        AsyncCmdlet
    {
        [Parameter]
        [ValidateRange(1, 100)]
        public int Threads { get; set; } = 3;

        [Parameter]
        [ValidateRange(1, 100)]
        public int Steps { get; set; } = 10;
        
        [Parameter]
        [ValidateRange(0, 10)]
        public double Wait { get; set; } = 1;

        public override Task ProcessRecordAsync()
        {
            var tasks = Enumerable.Range(1, this.Threads)
                                  .Select(i => Task.Run(async () => await this.CountAsync(i)));

            var allTasks = Task.WhenAll(tasks.ToArray());
            return allTasks;
        }

        private async Task CountAsync(
            int activityId)
        {
            var progressRecord = new ProgressRecord(activityId, $"Thread {activityId} ({Thread.CurrentThread.ManagedThreadId})", "Counting");

            int steps = this.Steps;
            double wait = this.Wait;

            for (int i = 0; i < steps; ++i)
            {
                this.CancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(wait));

                progressRecord.PercentComplete = (int)((double)i * 100 / steps);

                await this.WriteProgressAsync(progressRecord);
            }

            progressRecord.RecordType = ProgressRecordType.Completed;

            await this.WriteProgressAsync(progressRecord);
        }
    }
}
