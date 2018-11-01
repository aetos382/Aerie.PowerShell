using System;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

using Aerie.PowerShell;

namespace Aerie.PowerSHell.Samples
{
    [Cmdlet("Test", "ParallelProgress")]
    public sealed class TestParallelProgressCommand :
        AsyncCmdlet
    {
        [Parameter]
        public int Threads { get; set; } = 3;

        [Parameter]
        public int Steps { get; set; } = 100;
        
        [Parameter]
        public double Wait { get; set; } = 0.5;

        public override Task ProcessRecordAsync()
        {
            // var tasks = Enumerable.Range(1, this.Threads).Select(i => Task.Run(async () => await this.CountAsync(i)));
            var tasks = Enumerable.Range(1, this.Threads).Select(i => this.CountAsync(i, this.CancellationToken));

            var allTasks = Task.WhenAll(tasks.ToArray());
            return allTasks;
        }

        private async Task CountAsync(
            int activityId,
            CancellationToken cancellationToken)
        {
            var progressRecord = new ProgressRecord(activityId, "Couting", $"Thread {activityId}");

            for (int i = 0; i < this.Steps; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(this.Wait));

                progressRecord.PercentComplete = (int)((double)i * 100 / this.Steps);

                await this.WriteProgressAsync(progressRecord);
            }

            progressRecord.RecordType = ProgressRecordType.Completed;

            await this.WriteProgressAsync(progressRecord);
        }
    }
}
