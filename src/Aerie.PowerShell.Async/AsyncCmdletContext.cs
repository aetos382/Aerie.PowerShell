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

        private delegate Task AsyncPipelineDelegate(IAsyncCmdlet cmdlet, CancellationToken cancellationToken);

        [NotNull]
        private static readonly AsyncPipelineDelegate _beginProcessingAsyncDelegate;

        [NotNull]
        private static readonly AsyncPipelineDelegate _processRecordAsyncDelegate;

        [NotNull]
        private static readonly AsyncPipelineDelegate _endProcessingAsyncDelegate;

        [NotNull]
        private readonly ScopedReaderWriterLock _scopeLock = new ScopedReaderWriterLock();

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

        public void DoBeginProcessingAsync()
        {
            this.ProcessOperationQueue(cancellationToken => _beginProcessingAsyncDelegate(this._cmdlet, cancellationToken));
        }

        public void DoProcessRecordAsync()
        {
            this.ProcessOperationQueue(cancellationToken => _processRecordAsyncDelegate(this._cmdlet, cancellationToken));
        }

        public void DoEndProcessingAsync()
        {
            this.ProcessOperationQueue(cancellationToken => _endProcessingAsyncDelegate(this._cmdlet, cancellationToken));
        }

        public void Dispose()
        {
            this._scopeLock.Dispose();
            _contexts.Remove(this._cmdlet);
        }

        public void Cancel()
        {
            using (this._scopeLock.BeginReadLockScope())
            {
                this._scope.Cancel();
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
            [NotNull] Func<CancellationToken, Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            using (var scope = this.BeginAsyncScope())
            {
                var task = func(scope.CancellationToken);

                task = task.ContinueWith(t => {
                        scope.CloseQueue();
                        t.Wait(); // 例外を外へ飛ばす
                    },
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Current);

                foreach (var action in scope.GetQueuedTasks())
                {
                    try
                    {
                        action.RunSynchronously();
                    }
                    catch (InvalidOperationException)
                    {
                        if (action.IsCanceled)
                        {
                            continue;
                        }

                        throw;
                    }
                }

                try
                {
                    task.Wait();
                }
                catch (AggregateException ex)
                {
                    var exceptions = ex.Flatten();

                    if (exceptions.InnerExceptions.Count == 1)
                    {
                        throw exceptions.InnerExceptions[0];
                    }

                    throw;
                }
            }
        }

        public Task QueueAsyncOperation(
            Action action,
            CancellationToken cancellationToken)
        {
            using (this._scopeLock.BeginReadLockScope())
            {
                var scope = this._scope;

                if (scope == null)
                {
                    throw new InvalidOperationException();
                }

                return scope.QueueAsyncOperation(action, cancellationToken);
            }
        }

        [CanBeNull]
        private AsyncCmdletScope _scope;

        public void EndScope()
        {
            using (this._scopeLock.BeginWriteLockScope())
            {
                this._scope = null;
            }
        }

        [NotNull]
        public AsyncCmdletScope BeginAsyncScope()
        {
            using (this._scopeLock.BeginWriteLockScope())
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
        private static AsyncPipelineDelegate CreateDelegate(
            [NotNull] string methodName)
        {
            var method = typeof(IAsyncCmdlet).GetMethod(methodName);

            var cmdletParameter = Expression.Parameter(typeof(IAsyncCmdlet));
            var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken));

            var lambdaExpression = Expression.Lambda<AsyncPipelineDelegate>(
                Expression.Call(
                    cmdletParameter,
                    method,
                    cancellationTokenParameter),
                cmdletParameter,
                cancellationTokenParameter);

            var @delegate = lambdaExpression.Compile();
            return @delegate;
        }
    }
}
