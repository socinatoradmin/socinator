#region

using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Serilog.Core;
using Serilog.Events;
using System;

#endregion

namespace DominatorHouseCore.Diagnostics.LogHelper
{
    //[Target("NlogUiTarget")]
    //public class NlogUiTarget : TargetWithLayout
    //{
    //    private readonly ILogViewModel _logViewModel;

    //    public NlogUiTarget()
    //    {
    //        _logViewModel = InstanceProvider.GetInstance<ILogViewModel>();
    //    }
    //    protected override void Write(LogEventInfo logEvent)
    //    {
    //        _logViewModel.Add(logEvent.FormattedMessage, logEvent.Level);
    //    }
    //}

    public class UiLogSink : ILogEventSink
    {
        private readonly ILogViewModel _logViewModel;
        private readonly IFormatProvider _formatProvider;

        public UiLogSink(IFormatProvider formatProvider = null)
        {
            _logViewModel = InstanceProvider.GetInstance<ILogViewModel>();
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            _logViewModel.Add(message, MapLogLevel(logEvent.Level));
        }

        private LogLevel MapLogLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Debug:
                    return LogLevel.Debug;

                case LogEventLevel.Information:
                    return LogLevel.Info;

                case LogEventLevel.Warning:
                    return LogLevel.Warn;

                case LogEventLevel.Error:
                    return LogLevel.Error;

                case LogEventLevel.Fatal:
                    return LogLevel.Fatal;

                default:
                    return LogLevel.Trace;
            }
        }
    }
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}