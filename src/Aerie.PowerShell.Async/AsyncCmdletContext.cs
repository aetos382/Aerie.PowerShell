using System;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal class AsyncCmdletContext :
        IDisposable
    {
        [NotNull]
        private static readonly ConditionalWeakTable<Cmdlet, AsyncCmdletContext> _contexts = new ConditionalWeakTable<Cmdlet, AsyncCmdletContext>();

        [NotNull]
        private readonly Cmdlet _cmdlet;

        [NotNull]
        private readonly Func<Cmdlet, Task> _beginProcessingAsyncDelegate;

        [NotNull]
        private readonly Func<Cmdlet, Task> _processRecordAsyncDelegate;

        [NotNull]
        private readonly Func<Cmdlet, Task> _endProcessingAsyncDelegate;

        private AsyncCmdletContext(
            [NotNull] Type cmdletType,
            [NotNull] Cmdlet cmdlet)
        {
            this._cmdlet = cmdlet;

            this._beginProcessingAsyncDelegate = CreateDelegate(cmdletType, nameof(IAsyncCmdlet.BeginProcessingAsync));
            this._processRecordAsyncDelegate = CreateDelegate(cmdletType, nameof(IAsyncCmdlet.ProcessRecordAsync));
            this._endProcessingAsyncDelegate = CreateDelegate(cmdletType, nameof(IAsyncCmdlet.EndProcessingAsync));
        }

        [NotNull]
        public Task DoBeginProcessingAsync(
            [NotNull] Cmdlet cmdlet)
        {
            return this._beginProcessingAsyncDelegate(cmdlet);
        }

        [NotNull]
        public Task DoProcessRecordAsync(
            [NotNull] Cmdlet cmdlet)
        {
            return this._processRecordAsyncDelegate(cmdlet);
        }

        [NotNull]
        public Task DoEndProcessingAsync(
            [NotNull] Cmdlet cmdlet)
        {
            return this._endProcessingAsyncDelegate(cmdlet);
        }

        [NotNull]
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public void Dispose()
        {
            this._cancellationTokenSource.Dispose();

            _contexts.Remove(this._cmdlet);
        }

        public void Cancel()
        {
            this._cancellationTokenSource.Cancel();
        }

        public CancellationToken CancellationToken
        {
            get
            {
                return this._cancellationTokenSource.Token;
            }
        }

        [NotNull]
        public static AsyncCmdletContext GetContext<TCmdlet>(
            [NotNull] TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            return _contexts.GetValue(cmdlet, c => new AsyncCmdletContext(typeof(TCmdlet), c));
        }

        [NotNull]
        private static Func<Cmdlet, Task> CreateDelegate(
            [NotNull] Type cmdletType,
            [NotNull] string methodName)
        {
            var method = cmdletType.GetMethod(methodName);

            var parameter = Expression.Parameter(typeof(Cmdlet));

            var lambdaExpression = Expression.Lambda<Func<Cmdlet, Task>>(
                Expression.Call(
                    Expression.Convert(
                        parameter,
                        cmdletType),
                    method),
                parameter);

            var @delegate = lambdaExpression.Compile();
            return @delegate;
        }
    }
}
