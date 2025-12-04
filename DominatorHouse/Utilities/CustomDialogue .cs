using DominatorHouseCore.Interfaces;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace DominatorHouse.Utilities
{
    internal class CustomDialogue: ICustomDialogue
    {
        public Window GetWindow(string title, object windowContent, bool WithoutClose = false)
        {
            var MetroWindow = new CustomWindow(title,windowContent, WithoutClose)
            {
                ShowInTaskbar = true,
                ShowActivated = true,
                Topmost = false,
                Owner = Application.Current.MainWindow,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                BorderThickness = new Thickness(0),
            };
            return MetroWindow;
        }

        public MessageDialogResult ShowCustomDialog(string title, string message, string affirmativeText, string negativeText)
        {
            Application.Current.MainWindow.Opacity = 0.3;
            var dialog = new CustomDialogWindow(title, message, affirmativeText, negativeText);
            dialog.Closing += (s, e) => Application.Current.MainWindow.Opacity = 1;
            dialog.ShowActivated = true;
            dialog.ShowDialog();
            return (bool)dialog.DialogResult ?
                MessageDialogResult.Affirmative
                : MessageDialogResult.Canceled;
        }

        public MessageDialogResult ShowDialog(string title, string message)
        {
            Application.Current.MainWindow.Opacity = 0.3;
            var dialog = new CustomDialogWindow(title, message);
            dialog.ShowActivated = true;
            dialog.Closing += (s, e) => Application.Current.MainWindow.Opacity = 1;
            dialog.ShowDialog();
            return (bool)dialog.DialogResult ?
                MessageDialogResult.Affirmative
                : MessageDialogResult.Canceled;
        }

        public string ShowModalInputExternal(string title, string message, string affirmativeText, string negativeText, string DefaultText)
        {
            var dialog = new CustomInputDialog(title,message,affirmativeText,negativeText,DefaultText);
            Application.Current.MainWindow.Opacity = 0.3;
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowActivated = true;
            dialog.Closing += (s, e) => Application.Current.MainWindow.Opacity = 1;
            dialog.ShowDialog();
            return dialog.DefaultText;
        }
    }
}
