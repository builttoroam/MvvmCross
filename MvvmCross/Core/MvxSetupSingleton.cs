// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MvvmCross.Base;
using MvvmCross.Exceptions;
using MvvmCross.Logging;

namespace MvvmCross.Core
{
    public abstract class MvxSetupSingleton
       : MvxSingleton<MvxSetupSingleton>
    {
        private static readonly object LockObject = new object();
        private static readonly object CompletionLockObject = new object();
        private static TaskCompletionSource<bool> IsInitialisedTaskCompletionSource;
        private IMvxSetup _setup;
        private IMvxSetupMonitor _currentMonitor;

        protected virtual IMvxSetup Setup => _setup;

        public virtual TMvxSetup PlatformSetup<TMvxSetup>()
            where TMvxSetup : IMvxSetup
        {
            try
            {
                return (TMvxSetup)Setup;
            }
            catch (Exception ex)
            {
                MvxLog.Instance.Error(ex, "Unable to cast setup to {0}", typeof(TMvxSetup));
                throw ex;
            }
        }

        protected static TMvxSetupSingleton EnsureSingletonAvailable<TMvxSetupSingleton>()
           where TMvxSetupSingleton : MvxSetupSingleton, new()
        {
            if (Instance != null)
                return Instance as TMvxSetupSingleton;

            lock (LockObject)
            {
                if (Instance != null)
                    return Instance as TMvxSetupSingleton;

                var instance = new TMvxSetupSingleton();
                instance.CreateSetup();
                return Instance as TMvxSetupSingleton;
            }
        }

        public virtual void EnsureInitialized()
        {
            // This method is safe to call multiple times - will only run init once
            StartSetupInitialization();

            // Return immediately if the task is completed
            if (IsInitialisedTaskCompletionSource.Task.IsCompleted)
                return;

            // Block waiting on Initialize to be completed
            IsInitialisedTaskCompletionSource.Task.GetAwaiter().GetResult();
        }

        public virtual void InitializeAndMonitor(IMvxSetupMonitor setupMonitor)
        {
            // Grab the lock for the setupMonitor to make sure
            // InitializationComplete isn't attempted whilst we're 
            // checking to see if Initialize has completed
            lock (CompletionLockObject)
            {
                var hasCompleted = false;
                // Grab the lock for setting the completion state after Initialize is complete
                lock (LockObject)
                {
                    hasCompleted = IsInitialisedTaskCompletionSource?.Task.IsCompleted ?? false;
                    // At this point if the completion state isn't completed, we know
                    // that we can set the _currentMonitor, and that InitializationComplete will get invoked
                    if (!hasCompleted)
                    {
                        _currentMonitor = setupMonitor;
                    }
                }

                // At this point, if completion state is completed, we should
                // NOT set the _currentMonitor (as there is a risk InitializationComplete
                // has already been invoked). Instead we should just call InitializationComplete
                // directly, and return.
                if (hasCompleted)
                {
                    setupMonitor.InitializationComplete();
                    return;
                }
            }

            // If we get here, we should attempt to start Initialize because all we
            // know is that Initialize hasn't completed. StartSetupInitialization is
            // clever enough to know if it needs to run Initialize, or if it's already running
            StartSetupInitialization();
        }

        public virtual void CancelMonitor(IMvxSetupMonitor setupMonitor)
        {
            lock (CompletionLockObject)
            {
                if (setupMonitor != _currentMonitor)
                {
                    throw new MvxException("The specified IMvxSetupMonitor is not the one registered in MvxSetupSingleton");
                }
                _currentMonitor = null;
            }
        }

        protected virtual void CreateSetup()
        {
            try
            {
                _setup = MvxSetup.Instance();
            }
            catch (Exception exception)
            {
                throw exception.MvxWrap("Failed to create setup instance");
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                lock (CompletionLockObject)
                {
                    _currentMonitor = null;
                }
            }
            base.Dispose(isDisposing);
        }

        private void StartSetupInitialization()
        {
            // Do double-test to detect if Initialize has started
            if (IsInitialisedTaskCompletionSource != null) return;
            lock (LockObject)
            {
                if (IsInitialisedTaskCompletionSource != null) return;

                // At this point we know Initialize hasn't started, so create 
                // the completion source and kick of Initialize
                IsInitialisedTaskCompletionSource = new TaskCompletionSource<bool>();
            }

            // InitializePrimary should be only init methods that needs to be done on the UI thread
            _setup.InitializePrimary();
            Task.Run(() =>
            {
                // InitializeSecondary should be the bulk of init methods (and done on non-UI thread)
                _setup.InitializeSecondary();
                lock (LockObject)
                {
                    IsInitialisedTaskCompletionSource.SetResult(true);
                }
                var dispatcher = Mvx.GetSingleton<IMvxMainThreadDispatcher>();
                dispatcher.RequestMainThreadAction(() =>
                {
                    lock (CompletionLockObject)
                    {
                        _currentMonitor?.InitializationComplete();
                    }
                });
            });
        }
    }
}
