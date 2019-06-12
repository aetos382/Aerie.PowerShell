using System;
using System.Threading;

using JetBrains.Annotations;

namespace Aerie.PowerShell.Async
{
    internal sealed class ScopedReaderWriterLock :
        ReaderWriterLockSlim
    {
        public ScopedReaderWriterLock()
            : base()
        {
        }

        public ScopedReaderWriterLock(
            LockRecursionPolicy recursionPolicy)
            : base(recursionPolicy)
        {
        }

        public IDisposable BeginReadLockScope()
        {
            this.EnterReadLock();

            return new LockScope(this.ExitReadLock);
        }

        public IDisposable BeginUpgradableReadLockScope()
        {
            this.EnterUpgradeableReadLock();

            return new LockScope(this.ExitUpgradeableReadLock);
        }

        public IDisposable BeginWriteLockScope()
        {
            this.EnterWriteLock();

            return new LockScope(this.ExitWriteLock);
        }

        private sealed class LockScope :
            IDisposable
        {
            [NotNull]
            private readonly Action _unlockAction;

            private bool _disposed;

            public LockScope(
                [NotNull] Action unlockAction)
            {
                this._unlockAction = unlockAction;
            }

            public void Dispose()
            {
                if (this._disposed)
                {
                    throw new ObjectDisposedException(nameof(LockScope));
                }

                this._unlockAction();
                this._disposed = true;
            }
        }
    }
}
