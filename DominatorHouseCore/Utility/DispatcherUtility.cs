using System;
using System.Windows;
using System.Windows.Threading;

namespace DominatorHouseCore.Utility
{
    public interface IDispatcherUtility
    {
        DispatcherOperation InvokeAsync(Action callback);
        void Invoke(Action callback);
    }

    public class DispatcherUtility : IDispatcherUtility
    {
        public DispatcherOperation InvokeAsync(Action callback)
        {
            return Application.Current.Dispatcher.InvokeAsync(callback);
        }

        public void Invoke(Action callback)
        {
            Application.Current.Dispatcher.Invoke(callback);
        }
    }
}
