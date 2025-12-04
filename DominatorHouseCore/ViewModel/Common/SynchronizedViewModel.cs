#region

using System;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel.Common
{
    public abstract class SynchronizedViewModel : BindableBase
    {
        private readonly SemaphoreSlim _asyncSyncContext = new SemaphoreSlim(1, 1);
        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                SetProperty(ref _isRunning, value);
                OnPropertyChanged(nameof(IsIdle));
            }
        }

        public bool IsIdle => !_isRunning;

        public async Task ExecuteSynchronized<T1>(Func<T1, Task> asyncOperation, T1 input1)
        {
            await _asyncSyncContext.WaitAsync();
            try
            {
                IsRunning = true;
                await asyncOperation(input1);
            }
            finally
            {
                IsRunning = false;
                _asyncSyncContext.Release();
            }
        }
    }
}