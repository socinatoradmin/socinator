#region

using DominatorHouseCore.Interfaces;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#endregion

namespace DominatorHouseCore.Utility
{
    public class Dialog
    {
        public static Dialog Instance { get; private set; }= new Dialog();
        public Window GetMetroWindowWithOutClose(object window, string title)
        {
            var dialog = InstanceProvider.GetInstance<ICustomDialogue>();
            return dialog.GetWindow(title,window,true);
        }

        public Window GetMetroWindow(object window, string title)
        {
            var dialog = InstanceProvider.GetInstance<ICustomDialogue>();
            return dialog.GetWindow(title,window);
        }

        public static MessageDialogResult ShowDialog(string title, string message)
        {
            var dialog = InstanceProvider.GetInstance<ICustomDialogue>();
            return dialog.ShowDialog(title, message);
        }
        public static MessageDialogResult ShowDialog(object WINDOW, string title, string message)
        {
            var dialog = InstanceProvider.GetInstance<ICustomDialogue>();
            return dialog.ShowDialog(title, message);
        }
        public static MessageDialogResult ShowCustomDialog(string title, string message, string affirmativeText,
            string negativeText)
        {
            var dialog = InstanceProvider.GetInstance<ICustomDialogue>();
            return dialog.ShowCustomDialog(title, message, affirmativeText, negativeText);
        }

        public static void CloseDialog(object sender)
        {
            try
            {
                var parentWindow = Window.GetWindow((DependencyObject) sender);
                parentWindow?.Close();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static string GetInputDialog(string title, string message, string defaultText, string firstButtonContent,
            string secondButtonContent)
        {
            try
            {
                var dialog = InstanceProvider.GetInstance<ICustomDialogue>();
                return dialog.ShowModalInputExternal(title,message,firstButtonContent,secondButtonContent,defaultText);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return string.Empty;
            }
        }
    }
}