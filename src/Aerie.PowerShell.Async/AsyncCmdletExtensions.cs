using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    public static class AsyncCmdletExtensions
    {
        [NotNull]
        public static Task WriteObjectAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object value)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return WriteObjectAsync(cmdlet, value, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteObjectAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object value,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteObject(value),
                option,
                cancellationToken);
        }

        [NotNull]
        public static Task WriteObjectAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object value,
            bool enumerateCollection)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return WriteObjectAsync(cmdlet, value, enumerateCollection, AsyncOperationOption.None, CancellationToken.None);
        }
        
        [NotNull]
        public static Task WriteObjectAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object value,
            bool enumerateCollection,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteObject(value, enumerateCollection),
                option,
                cancellationToken);
        }
        
        [NotNull]
        public static Task WriteErrorAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ErrorRecord errorRecord)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (errorRecord == null)
            {
                throw new ArgumentNullException(nameof(errorRecord));
            }

            return WriteErrorAsync(cmdlet, errorRecord, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteErrorAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ErrorRecord errorRecord,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (errorRecord == null)
            {
                throw new ArgumentNullException(nameof(errorRecord));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteError(errorRecord),
                option,
                cancellationToken);
        }
        
        [NotNull]
        public static Task WriteWarningAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return WriteWarningAsync(cmdlet, message, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteWarningAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteWarning(message),
                option,
                cancellationToken);
        }
        
        [NotNull]
        public static Task WriteVerboseAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return WriteVerboseAsync(cmdlet, message, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteVerboseAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteVerbose(message),
                option,
                cancellationToken);
        }
       
        [NotNull]
        public static Task WriteDebugAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return WriteDebugAsync(cmdlet, message, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteDebugAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteDebug(message),
                option,
                cancellationToken);
        }
        
        [NotNull]
        public static Task WriteProgressAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ProgressRecord progressRecord)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (progressRecord == null)
            {
                throw new ArgumentNullException(nameof(progressRecord));
            }

            return WriteProgressAsync(cmdlet, progressRecord, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteProgressAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ProgressRecord progressRecord,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (progressRecord == null)
            {
                throw new ArgumentNullException(nameof(progressRecord));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteProgress(progressRecord),
                option,
                cancellationToken);
        }
                            
        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] InformationRecord informationRecord)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (informationRecord == null)
            {
                throw new ArgumentNullException(nameof(informationRecord));
            }

            return WriteInformationAsync(cmdlet, informationRecord, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] InformationRecord informationRecord,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            if (informationRecord == null)
            {
                throw new ArgumentNullException(nameof(informationRecord));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteInformation(informationRecord),
                option,
                cancellationToken);
        }
                           
        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object messageData,
            [CanBeNull] string[] tags)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return WriteInformationAsync(cmdlet, messageData, tags, AsyncOperationOption.None, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object messageData,
            [CanBeNull] string[] tags,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(
                cmdlet,
                () => cmdlet.WriteInformation(messageData, tags),
                option,
                cancellationToken);
        }

        public static void DoBeginProcessingAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            var context = AsyncCmdletContext.GetContext(cmdlet);
            context.DoBeginProcessingAsync();
        }

        public static void DoProcessRecordAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            var context = AsyncCmdletContext.GetContext(cmdlet);
            context.DoProcessRecordAsync();
        }
        
        public static void DoEndProcessingAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            var context = AsyncCmdletContext.GetContext(cmdlet);
            context.DoEndProcessingAsync();
        }

        public static void DoStopProcessing<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            try
            {
                var context = AsyncCmdletContext.GetContext(cmdlet);
                context.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public static void DoDispose<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            var context = AsyncCmdletContext.GetContext(cmdlet);
            context.Dispose();
        }

        public static CancellationToken GetCancellationToken<TCmdlet>(
            [NotNull] this TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            var context = AsyncCmdletContext.GetContext(cmdlet);
            return context.CancellationToken;
        }
        
        [NotNull]
        private static Task PostAsyncOperation<TCmdlet>(
            [NotNull] TCmdlet cmdlet,
            [NotNull] Action action,
            AsyncOperationOption option,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            var context = AsyncCmdletContext.GetContext(cmdlet);
            var task = context.QueueAsyncOperation(action, option, cancellationToken);

            return task;
        }
    }
}
