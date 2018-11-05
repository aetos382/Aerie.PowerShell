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

            return WriteObjectAsync(cmdlet, value, false, false, CancellationToken.None);
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

            return WriteObjectAsync(cmdlet, value, enumerateCollection, false, CancellationToken.None);
        }
        
        [NotNull]
        public static Task WriteObjectAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object value,
            bool enumerateCollection,
            bool executeSynchronously,
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
                executeSynchronously,
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

            return WriteErrorAsync(cmdlet, errorRecord, false, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteErrorAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ErrorRecord errorRecord,
            bool executeSynchronously,
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
                executeSynchronously,
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

            return WriteWarningAsync(cmdlet, message, false, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteWarningAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            bool executeSynchronously,
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
                executeSynchronously,
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

            return WriteVerboseAsync(cmdlet, message, false, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteVerboseAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            bool executeSynchronously,
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
                executeSynchronously,
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

            return WriteDebugAsync(cmdlet, message, false, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteDebugAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            string message,
            bool executeSynchronously,
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
                executeSynchronously,
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

            return WriteProgressAsync(cmdlet, progressRecord, false, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteProgressAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] ProgressRecord progressRecord,
            bool executeSynchronously,
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
                executeSynchronously,
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

            return WriteInformationAsync(cmdlet, informationRecord, false, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [NotNull] InformationRecord informationRecord,
            bool executeSynchronously,
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
                executeSynchronously,
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

            return WriteInformationAsync(cmdlet, messageData, tags, false, CancellationToken.None);
        }

        [NotNull]
        public static Task WriteInformationAsync<TCmdlet>(
            [NotNull] this TCmdlet cmdlet,
            [CanBeNull] object messageData,
            [CanBeNull] string[] tags,
            bool executeSynchronously,
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
                executeSynchronously,
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
            bool executeSynchronously,
            CancellationToken cancellationToken)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            var context = AsyncCmdletContext.GetContext(cmdlet);
            var task = context.QueueAsyncOperation(action, executeSynchronously, cancellationToken);

            return task;
        }
    }
}
