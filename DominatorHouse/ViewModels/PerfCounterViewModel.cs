using DominatorHouseCore.Utility;
using Prism.Commands;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace DominatorHouse.ViewModels
{
    public interface IPerfCounterViewModel : IDisposable
    {

    }

    public class PerfCounterViewModel : BindableBase, IPerfCounterViewModel
    {
        private readonly IPerfCounterService _perfCounterService;
        private readonly DispatcherTimer _timer;
        private GridLength _logViewHeight;
        private string _cpuUsage;
        private string _availableMemory;
        private DateTime? _currentDateTime;
        private string _loadedMemory;
        public string LoadedMemory
        {
            get { return _loadedMemory; }
            set { SetProperty(ref _loadedMemory, value, nameof(LoadedMemory)); }
        }
        public string AvailableMemory
        {
            get { return _availableMemory; }
            set { SetProperty(ref _availableMemory, value, nameof(AvailableMemory)); }
        }

        public string CpuUsage
        {
            get { return _cpuUsage; }
            set { SetProperty(ref _cpuUsage, value, nameof(CpuUsage)); }
        }

        public DateTime? CurrentDateTime
        {
            get { return _currentDateTime; }
            set { SetProperty(ref _currentDateTime, value, nameof(CurrentDateTime)); }
        }

        public GridLength LogViewHeight
        {
            get { return _logViewHeight; }
            set { SetProperty(ref _logViewHeight, value, nameof(LogViewHeight)); }
        }

        public DelegateCommand ShowHideLogCmd { get; }

        public PerfCounterViewModel(IPerfCounterService perfCounterService)
        {
            _perfCounterService = perfCounterService;
            LoadedMemory = _perfCounterService.LoadedMemoryDescrption;
            LogViewHeight = new GridLength(3, GridUnitType.Star);
            ShowHideLogCmd = new DelegateCommand(ShowHideLog);
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _timer.Tick += OnElapsed;
            _timer.Start();
        }

        private void OnElapsed(object sender, EventArgs e)
        {
            var counters = _perfCounterService.GetActualValues();
            AvailableMemory = counters.AvailableMemory.ToString(CultureInfo.InvariantCulture);
            CpuUsage = counters.CpuUsage.ToString(CultureInfo.InvariantCulture);
            CurrentDateTime = DateTime.Now;
        }

        private void ShowHideLog()
        {
            if (LogViewHeight.Value <= 200 && LogViewHeight.Value > 45)
                LogViewHeight = new GridLength(45);
            else
                LogViewHeight = new GridLength(200);
        }

        public void Dispose()
        {
            _timer.Tick -= OnElapsed;
            _timer.Stop();
        }
    }
}
