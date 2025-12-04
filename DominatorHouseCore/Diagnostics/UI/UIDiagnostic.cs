#region

using System;
using System.Windows;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.Diagnostics
{
    internal static class UIDiagnostic
    {
        public static void Warning(string format, params object[] args)
        {
            var message = string.Format(format, args);
            GlobusLogHelper.log.Warn(message);
            MessageBox.Show(message, "LangKeyWarning".FromResourceDictionary(), MessageBoxButton.OK,
                MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        public static void Warning(string message)
        {
            GlobusLogHelper.log.Warn(message);
            MessageBox.Show(message, "LangKeyWarning".FromResourceDictionary(), MessageBoxButton.OK,
                MessageBoxImage.Warning, MessageBoxResult.OK);
        }

        public static void Error(string format, params object[] args)
        {
            var message = string.Format(format, args);
            GlobusLogHelper.log.Error(message);
            //MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        public static void Error(string message)
        {
            GlobusLogHelper.log.Error(message);
            //MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        public static void Error(Exception ex, string message)
        {
            Error($"{message}\r\n{ex}");
        }

        public static void Error(Exception ex)
        {
            Error(ex.ToString());
        }

        public static void Info(string format, params object[] args)
        {
            var message = string.Format(format, args);
            GlobusLogHelper.log.Info(message);
            //MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        public static void Info(string message)
        {
            GlobusLogHelper.log.Info(message);
            //MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }

        public static void Fatal(Exception ex, string format, params object[] args)
        {
            var fullMsg = ex.ToUserString(format, args);
            Fatal(fullMsg);
        }

        public static void Fatal(string format, params object[] args)
        {
            var message = string.Format(format, args);

            GlobusLogHelper.log.Error(message);
            var appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            message += "\r\n\r\n" + string.Format("LangKeyFatalErrorMessage".FromResourceDictionary(), appName);
            GlobusLogHelper.log.Error("Fatal Exit...");
            GlobusLogHelper.log.Error(message);

            //try
            //{
            //    Environment.Exit(1);
            //}
            //catch
            //{
            //    System.Diagnostics.Process.GetCurrentProcess().Kill();
            //}
        }

        public static bool Ask(string format, params object[] args)
        {
            var message = string.Format(format, args);
            GlobusLogHelper.log.Info(message);
            var answer = MessageBox.Show(message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Information,
                MessageBoxResult.No);
            return answer == MessageBoxResult.Yes;
        }

        public static bool Ask(string message, string title = "Question")
        {
            GlobusLogHelper.log.Info(message);
            var answer = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Information,
                MessageBoxResult.No);
            return answer == MessageBoxResult.Yes;
        }
    }
}