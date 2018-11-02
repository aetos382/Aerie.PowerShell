using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aerie.PowerShell
{
    public interface IAsyncCmdlet :
        IDisposable
    {
        Task BeginProcessingAsync();

        Task ProcessRecordAsync();

        Task EndProcessingAsync();
    }
}
