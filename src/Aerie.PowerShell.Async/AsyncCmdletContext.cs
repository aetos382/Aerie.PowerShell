using System;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Aerie.PowerShell.Async
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
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private delegate Task AsyncPipelineDelegate(IAsyncCmdlet cmdlet);

        [NotNull]
        private static readonly AsyncPipelineDelegate _beginProcessingAsyncDelegate;

        [NotNull]
        private static readonly AsyncPipelineDelegate _processRecordAsyncDelegate;

        [NotNull]
        private static readonly AsyncPipelineDelegate _endProcessingAsyncDelegate;

        [NotNull]
        private readonly ScopedReaderWriterLock _scopeLock = new ScopedReaderWriterLock();

        private bool _disposed;

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
            this.CheckDisposed();

            this.ProcessOperationQueue(() => _beginProcessingAsyncDelegate(this._cmdlet));
        }

        public void DoProcessRecordAsync()
        {
            this.CheckDisposed();

            this.ProcessOperationQueue(() => _processRecordAsyncDelegate(this._cmdlet));
        }

        public void DoEndProcessingAsync()
        {
            this.CheckDisposed();

            this.ProcessOperationQueue(() => _endProcessingAsyncDelegate(this._cmdlet));
        }

        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._scopeLock.Dispose();
            this._cancellationTokenSource.Dispose();

            _contexts.Remove(this._cmdlet);

            this._disposed = true;
        }

        public void Cancel()
        {
            this.CheckDisposed();

            this._cancellationTokenSource.Cancel();
        }

        public CancellationToken CancellationToken
        {
            get
            {
                this.CheckDisposed();

                return this._cancellationTokenSource.Token;
            }
        }

        [NotNull]
        public static AsyncCmdletContext GetContext<TCmdlet>(
            [NotNull] TCmdlet cmdlet)
            where TCmdlet : Cmdlet, IAsyncCmdlet
        {
            return _contexts.GetValue(cmdlet, c => new AsyncCmdletContext(c));
        }

        private void ProcessOperationQueue(
            [NotNull] Func<Task> func)
        {
            if (this.CancellationToken.IsCancellationRequested)
            {
                return;
            }

            using (var scope = this.BeginAsyncScope())
            {
                var task = func();

                task = task.ContinueWith(t =>
                    {
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
                    this.Cancel();

                    var exceptions = ex.Flatten();

                    if (exceptions.InnerExceptions.Count == 1)
                    {
                        throw exceptions.InnerExceptions[0];
                    }

                    throw;
                }
                catch
                {
                    this.Cancel();
                    throw;
                }
            }
        }

        public Task QueueAsyncOperation(
            Action action,
            bool executeSynchronously,
            CancellationToken cancellationToken)
        {
            this.CheckDisposed();

            using (this._scopeLock.BeginReadLockScope())
            {
                var scope = this._scope;

                if (scope is null)
                {
                    throw new InvalidOperationException();
                }

                return scope.RequestAsyncOperation(action, executeSynchronously, cancellationToken);
            }
        }

        [CanBeNull]
        private AsyncCmdletScope _scope;

        public void EndScope()
        {
            this.CheckDisposed();

            using (this._scopeLock.BeginWriteLockScope())
            {
                this._scope = null;
            }
        }

        [NotNull]
        private AsyncCmdletScope BeginAsyncScope()
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

            var lambdaExpression = Expression.Lambda<AsyncPipelineDelegate>(
                Expression.Call(
                    cmdletParameter,
                    method),
                cmdletParameter);

            var @delegate = lambdaExpression.Compile();
            return @delegate;
        }

        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(nameof(AsyncCmdletContext));
            }
        }
    }
}
