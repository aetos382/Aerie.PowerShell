using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aerie.PowerShell
{
    public interface IAsyncCmdlet :
        IDisposable
    {
        Task BeginProcessingAsync(
            CancellationToken cancellationToken);

        Task ProcessRecordAsync(
            CancellationToken cancellationToken);

        Task EndProcessingAsync(
            CancellationToken cancellationToken);
    }
}
