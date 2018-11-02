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

            return WriteObjectAsync(cmdlet, value, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteObjectAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object value,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteObject(value), cancellationToken);
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

            return WriteObjectAsync(cmdlet, value, enumerateCollection, CancellationToken.None);
        }
        
        [NotNull]
        public static Task WriteObjectAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object value,
            bool enumerateCollection,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteObject(value, enumerateCollection), cancellationToken);
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

            return WriteErrorAsync(cmdlet, errorRecord, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteErrorAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ErrorRecord errorRecord,
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

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteError(errorRecord), cancellationToken);
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

            return WriteWarningAsync(cmdlet, message, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteWarningAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteWarning(message), cancellationToken);
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

            return WriteVerboseAsync(cmdlet, message, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteVerboseAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteVerbose(message), cancellationToken);
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

            return WriteDebugAsync(cmdlet, message, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteDebugAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteDebug(message), cancellationToken);
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

            return WriteProgressAsync(cmdlet, progressRecord, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteProgressAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ProgressRecord progressRecord,
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

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteProgress(progressRecord), cancellationToken);
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

            return WriteInformationAsync(cmdlet, informationRecord, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] InformationRecord informationRecord,
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

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteInformation(informationRecord), cancellationToken);
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

            return WriteInformationAsync(cmdlet, messageData, tags, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object messageData,
            [CanBeNull] string[] tags,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException(nameof(cmdlet));
            }

            return PostAsyncOperation(cmdlet, () => cmdlet.WriteInformation(messageData, tags), cancellationToken);
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
        
        [NotNull]
        private static Task PostAsyncOperation<TCmdlet>(
            [NotNull] TCmdlet cmdlet,
            [NotNull] Action action,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            var context = AsyncCmdletContext.GetContext(cmdlet);
            var task = context.QueueAsyncOperation(action, cancellationToken);

            return task;
        }
    }
}
