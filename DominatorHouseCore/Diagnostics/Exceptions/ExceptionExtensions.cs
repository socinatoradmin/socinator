#region

using System;
using DominatorHouseCore.LogHelper;

#endregion

namespace DominatorHouseCore
{
    public static class ExceptionExtensions
    {
        ///// <summary>
        ///// Extension method which converts exception to Type+Message+StackTrace string
        ///// </summary>
        ///// <param name="ex"></param>
        ///// <returns></returns>
        //public static string ToUnhandledString(this Exception ex)
        //{
        //    return string.Format("Unhandled exception of type '{0}' has been thrown\r\n\r\nMessage: {1}\r\n\r\nStack Trace:\r\n{2}",
        //        ex.GetType(), ex.Message, ex.StackTrace);
        //}


        /// <summary>
        ///     Extension method of exception that converts it to Type+Message+UserMessage
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="userMessage"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string ToUserString(this Exception ex, string userMessage, params object[] args)
        {
            return string.Format(
                "Exception of type '{0}' has been thrown\r\n\r\nMessage: {1}\r\n\r\nMessage Details: {2}",
                ex.GetType(), ex.Message, string.Format(userMessage, args));
        }

        public static string ToUserStringWithStack(this Exception ex, string userMessage, params object[] args)
        {
            return string.Format("Exception of type '{0}' has been thrown\r\n\r\nMessage: {1}" +
                                 "\r\n\r\nMessage Details: {2}\r\n\r\nStack Trace: {3}",
                ex.GetType(), ex.Message, string.Format(userMessage, args), ex.StackTrace);
        }

        public static void TraceLog(this Exception ex)
        {
            TraceLog(ex, "");
        }

        public static void TraceLog(this Exception ex, string userMessage, params object[] args)
        {
            GlobusLogHelper.log.Trace(ex.ToUserStringWithStack(userMessage, args));
        }


        public static void DebugLog(this Exception ex)
        {
            DebugLog(ex, "");
        }

        public static void DebugLog(this Exception ex, string userMessage, params object[] args)
        {
            GlobusLogHelper.log.Debug(ex.ToUserStringWithStack(userMessage, args));
        }

        public static void ErrorLog(this Exception ex)
        {
            ex.DebugLog();
            // ErrorLog(ex, "");
        }

        public static void ErrorLog(this Exception ex, string userMessage, params object[] args)
        {
            ex.DebugLog();
            //GlobusLogHelper.log.Error(ex.ToUserStringWithStack(userMessage, args));
        }
    }
}