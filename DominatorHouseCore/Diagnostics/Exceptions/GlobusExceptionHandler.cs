#region

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore.LogHelper;
using FluentScheduler;
using Registry = Microsoft.Win32.Registry;

#endregion

namespace DominatorHouseCore.Diagnostics.Exceptions
{
    internal class GlobusExceptionHandler
    {
        [DllImport("kernel32.dll")]
        private static extern ErrorModes SetErrorMode(ErrorModes uMode);

        [Flags]
        public enum ErrorModes : uint
        {
            //SYSTEM_DEFAULT = 0x0,
            //SEM_FAILCRITICALERRORS = 0x0001,
            //SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002
            //SEM_NOOPENFILEERRORBOX = 0x8000
        }

        // Call to disable error report dialogs over App
        public static void DisableErrorDialog()
        {
            try
            {
                var dwMode = SetErrorMode(ErrorModes.SEM_NOGPFAULTERRORBOX);
                SetErrorMode(dwMode | ErrorModes.SEM_NOGPFAULTERRORBOX);
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }
        }

        public static void SetupGlobalExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                try
                {
                    HandleGlobalException(e.ExceptionObject as Exception, o.ToString());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };
            Application.Current.Exit += (o, e) =>
            {
                GlobusLogHelper.log.Debug($"Application exit code {e.ApplicationExitCode}");
            };
            AppDomain.CurrentDomain.ProcessExit += (o, e) =>
            {
                GlobusLogHelper.log.Debug($"Application exit code {o.ToString()}");
            };
            Application.Current.DispatcherUnhandledException += (o, e) =>
            {
                e.Exception.DebugLog();
                e.Handled = true;
            };
            Application.Current.Dispatcher.UnhandledExceptionFilter += (o, e) => { e.Exception.DebugLog(); };
            TaskScheduler.UnobservedTaskException += (o, e) =>
            {
                try
                {
                    e.Exception.DebugLog();
                    e.SetObserved();
                    //HandleGlobalException(e.Exception, o.ToString());
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            // Exception within jobs
            JobManager.JobException += job =>
            {
                try
                {
                    job.Exception.DebugLog();
                    // HandleGlobalException(job.Exception, job.Name);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };
        }


        /// <summary>
        ///     Application will be exit after notifying user on Unhandled exception occurred
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="senderString"></param>
        internal static void HandleGlobalException(Exception exception, string senderString)
        {
            try
            {
                if (exception != null)
                    UIDiagnostic.Fatal(exception, "Unhandled exception has been thrown from {0}", senderString);
                else
                    UIDiagnostic.Fatal("Unhandled exception has been thrown from {0}", senderString);
            }
            catch (Exception ex)
            {
                UIDiagnostic.Fatal(ex, "Unhandled exception has been thrown in HandleGlobalException()");
            }
        }
    }
}