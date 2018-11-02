using System;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell
{
    internal sealed class AsyncCmdletContext :
        IDisposable
    {
        [NotNull]
        private static readonly ConditionalWeakTable<IAsyncCmdlet, AsyncCmdletContext> _contexts
            = new ConditionalWeakTable<IAsyncCmdlet, AsyncCmdletContext>();

        [NotNull]
        private readonly IAsyncCmdlet _cmdlet;

        [NotNull]
        private static readonly Func<IAsyncCmdlet, Task> _beginProcessingAsyncDelegate;

        [NotNull]
        private static readonly Func<IAsyncCmdlet, Task> _processRecordAsyncDelegate;

        [NotNull]
        private static readonly Func<IAsyncCmdlet, Task> _endProcessingAsyncDelegate;

        static AsyncCmdletContext()
        {
            _beginProcessingAsyncDelegate = CreateDelegate(nameof(IAsyncCmdlet.BeginProcessingAsync));
            _processRecordAsyncDelegate = CreateDelegate(nameof(IAsyncCmdlet.ProcessRecordAsync));
            _endProcessingAsyncDelegate = CreateDelegate(nameof(IAsyncCmdlet.EndProcessingAsync));
        }

        private AsyncCmdletContext(
            [NotNull] IAsyncCmdlet cmdlet)
        {
            this._cmdlet = cmdlet;
        }

        [NotNull]
        public Task DoBeginProcessingAsync()
        {
            return _beginProcessingAsyncDelegate(this._cmdlet);
        }

        [NotNull]
        public Task DoProcessRecordAsync()
        {
            return _processRecordAsyncDelegate(this._cmdlet);
        }

        [NotNull]
        public Task DoEndProcessingAsync()
        {
            return _endProcessingAsyncDelegate(this._cmdlet);
        }

        public void Dispose()
        {
            lock (this._scopeLock)
            {
                this._scope?.Dispose();
            }

            _contexts.Remove(this._cmdlet);
        }

        public void Cancel()
        {
            lock (this._scopeLock)
            {
                this._scope.Cancel();
            }
        }

        public CancellationToken CancellationToken
        {
            get
            {
                lock (this._scopeLock)
                {
                    return this._scope.CancellationToken;
                }
            }
        }

        [NotNull]
        public static AsyncCmdletContext GetContext<TCmdlet>(
            [NotNull] TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            return _contexts.GetValue(cmdlet, c => new AsyncCmdletContext(c));
        }

        public void ProcessOperationQueue(
            [NotNull] Func<Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            using (var scope = this.BeginAsyncScope())
            {
                var task = func();

                task = task.ContinueWith(t => {
                        scope.CloseQueue();
                        t.Wait(); // 例外を外へ飛ばす
                    },
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Current);

                foreach (var action in scope.GetQueuedTasks())
                {
                    action.RunSynchronously();
                }

                task.Wait();
            }
        }

        public Task QueueAsyncOperation(
            Action action,
            CancellationToken cancellationToken)
        {
            lock (this._scopeLock)
            {
                var scope = this._scope;

                if (scope == null)
                {
                    throw new InvalidOperationException();
                }

                return scope.QueueAsyncOperation(action, cancellationToken);
            }
        }

        [NotNull]
        private readonly object _scopeLock = new object();

        [CanBeNull]
        private AsyncCmdletScope _scope;

        public void EndScope()
        {
            lock (this._scopeLock)
            {
                this._scope = null;
            }
        }

        [NotNull]
        public AsyncCmdletScope BeginAsyncScope()
        {
            lock (this._scopeLock)
            {
                if (this._scope != null)
                {
                    throw new InvalidOperationException();
                }

                this._scope = new AsyncCmdletScope(this);
                return this._scope;
            }
        }

        [NotNull]
        private static Func<IAsyncCmdlet, Task> CreateDelegate(
            [NotNull] string methodName)
        {
            var method = typeof(IAsyncCmdlet).GetMethod(methodName);

            var cmdletParameter = Expression.Parameter(typeof(IAsyncCmdlet));

            var lambdaExpression = Expression.Lambda<Func<IAsyncCmdlet, Task>>(
                Expression.Call(
                    cmdletParameter,
                    method),
                cmdletParameter);

            var @delegate = lambdaExpression.Compile();
            return @delegate;
        }
    }
}
