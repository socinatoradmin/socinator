#region

using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class ToasterNotification
    {
        public static Notifier Notifier { get; }

        static ToasterNotification()
        {
            Notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    Application.Current.MainWindow,
                    Corner.BottomRight,
                    10,
                    10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(5),
                    MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }

        public static void ShowInfomation(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => { Notifier.ShowInformation(message); });
            else
                Notifier.ShowInformation(message);
        }

        public static void ShowSuccess(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => { Notifier.ShowSuccess(message); });
            else
                Notifier.ShowSuccess(message);
        }

        public static void ShowError(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Notifier.ShowError(message, new MessageOptions {FontSize = 14});
                });
            else
                Notifier.ShowError(message, new MessageOptions {FontSize = 14});
        }

        public static void ShowWarning(string message)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => { Notifier.ShowWarning(message); });
            else
                Notifier.ShowWarning(message);
        }

        public static void Dispose()
        {
            Notifier.Dispose();
        }
    }
}