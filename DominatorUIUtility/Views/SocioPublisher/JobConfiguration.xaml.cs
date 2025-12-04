using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for JobConfiguration.xaml
    /// </summary>
    public partial class JobConfiguration : UserControl
    {
        private static JobConfiguration _jobConfiguration;

        public JobConfiguration()
        {
            InitializeComponent();
            MainGrid.DataContext = this;
            JobConfigurations = new JobConfigurationModel();
        }


        private JobConfiguration(JobConfigurationModel jobConfigurationModel)
        {
            InitializeComponent();
            JobConfigurations = jobConfigurationModel;
            MainGrid.DataContext = JobConfigurations;
            BindingOperations.EnableCollectionSynchronization(JobConfigurations.LstTimer, _lock);
        }

        private bool _isEditMode { get; set; }

        public static JobConfiguration GetInstance(JobConfigurationModel jobConfigurationModel, bool isEditMode)
        {
            if (_jobConfiguration == null)
                _jobConfiguration = new JobConfiguration(jobConfigurationModel);
            _jobConfiguration._isEditMode = isEditMode;
            _jobConfiguration.CancelToken();
            // _jobConfiguration.LastPostCount = 0;
            _jobConfiguration.LastPostCount = jobConfigurationModel.LstTimer.Count;
            _jobConfiguration.JobConfigurations = jobConfigurationModel;
            _jobConfiguration.MainGrid.DataContext = _jobConfiguration.JobConfigurations;
            _jobConfiguration.OnPostCountChanged();
            return _jobConfiguration;
        }

        private void OnSelectedTimeChanged(object sender, TimePickerBaseSelectionChangedEventArgs<TimeSpan?> e)
        {
            //if (IsAllowEdit)
            //{
            //    if (JobConfigurations == null)
            //        return;

            //    if (JobConfigurations.IsSpecifyPostingIntervalChecked)
            //        SpecificPostGenerateIntervals(JobConfigurations.MaxPost);

            //    if (JobConfigurations.IsRandomizePublishingTimerChecked)
            //    {
            //        GenerateRandomIntervals(JobConfigurations.MaxPost);
            //    }
            //} 
        }

        private void CancelToken()
        {
            cancellectionToken?.Cancel();
            cancellectionToken?.Dispose();
            cancellectionToken = new CancellationTokenSource();
        }

        private void PostCountChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            OnPostCountChanged();
        }

        private void OnPostCountChanged()
        {
            try
            {
                if (_isEditMode)
                {
                    _isEditMode = false;
                    return;
                }

                CancelToken();
                JobConfigurations.LstTimer.Clear();
                if (/*numericMaxPost.Value != null &&*/ JobConfigurations.MaxPost != 0)
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost, cancellectionToken, true);
            }
            catch { }
        }

        #region Properties

        private readonly object _lock = new object();
        public JobConfigurationModel JobConfigurations { get; set; }
        private CancellationTokenSource cancellectionToken { get; set; }
        public int LastPostCount { get; set; }

        #endregion

        #region Post max count changed

        private void NumericMaxPost_OnValueDecremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            try
            {
                if (LastPostCount != JobConfigurations.MaxPost)
                    CancelToken();

                if (LastPostCount < JobConfigurations.MaxPost - 1)
                {
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1 - LastPostCount, cancellectionToken,
                        true);
                }
                else
                {
                    if (JobConfigurations.LstTimer.Count < JobConfigurations.MaxPost)
                        SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1 - JobConfigurations.LstTimer.Count,
                            cancellectionToken, true);

                    else
                        SpecificPostGenerateIntervals(JobConfigurations.MaxPost - 1, cancellectionToken, false);
                }

                LastPostCount = JobConfigurations.MaxPost - 1;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void NumericMaxPost_OnValueIncremented(object sender, NumericUpDownChangedRoutedEventArgs args)
        {
            if (LastPostCount != JobConfigurations.MaxPost)
                CancelToken();

            try
            {
                if (LastPostCount < JobConfigurations.MaxPost + 1)
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1 - LastPostCount, cancellectionToken,
                        true);
                else if (JobConfigurations.LstTimer.Count < JobConfigurations.MaxPost + 1)
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1 - JobConfigurations.LstTimer.Count,
                        cancellectionToken, true);

                else
                    SpecificPostGenerateIntervals(JobConfigurations.MaxPost + 1, cancellectionToken, false);

                LastPostCount = JobConfigurations.MaxPost + 1;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Specific Post Interval Generations

        private void SpecificPostGenerateIntervals(int maxCount, CancellationTokenSource cancellectionToken,
            bool isNeedToAdd)
        {
            var random = new Random();

            var startTime = JobConfigurations.TimeRange.StartTime;
            var endTime = JobConfigurations.TimeRange.EndTime;

            if (startTime > endTime)
            {
                JobConfigurations.LstTimer = new ObservableCollection<TimeSpanHelper>();
                GlobusLogHelper.log.Info("Start time should be greater than end time");
                return;
            }

            var totalSeconds = (int) (endTime - startTime).TotalSeconds;

            try
            {
                var timeRange = totalSeconds / maxCount;
                var timeToAddToStartTime = TimeSpan.FromSeconds(timeRange);
                if (isNeedToAdd)
                    AddTimeRange(maxCount, cancellectionToken, random, startTime, endTime, timeToAddToStartTime);
                else
                    RemoveTimeRange(maxCount, cancellectionToken);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Attempted to divide by zero.")
                    RemoveTimeRange(0, cancellectionToken);
            }
        }

        private void AddTimeRange(int maxCount, CancellationTokenSource cancellectionToken, Random random,
            TimeSpan startTime, TimeSpan endTime, TimeSpan timeToAddToStartTime)
        {
            Task.Factory.StartNew(() =>
            {
                for (var noOfPost = 0; noOfPost < maxCount; noOfPost++)
                {
                    if (!Constants.IsCancelationTokenDisposed(cancellectionToken))
                        cancellectionToken?.Token.ThrowIfCancellationRequested();
                    endTime = startTime + timeToAddToStartTime;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                       if(!Constants.IsCancelationTokenDisposed(cancellectionToken))
                             cancellectionToken?.Token.ThrowIfCancellationRequested();
                        JobConfigurations.LstTimer.Add(new TimeSpanHelper
                        {
                            StartTime = startTime,
                            MidTime = DateTimeUtilities.GetRandomTime(startTime, endTime, random),
                            EndTime = endTime
                        });
                    });
                    startTime = endTime + TimeSpan.FromSeconds(1);
                    Thread.Sleep(50);
                }
            });
        }


        private void RemoveTimeRange(int maxCount, CancellationTokenSource cancellectionToken)
        {
            Task.Factory.StartNew(() =>
            {
                while (JobConfigurations.LstTimer.Count > maxCount)
                {
                    if(cancellectionToken.Token.CanBeCanceled)
                        cancellectionToken.Token.ThrowIfCancellationRequested();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (cancellectionToken.Token.CanBeCanceled)
                            cancellectionToken.Token.ThrowIfCancellationRequested();
                        try
                        {
                            JobConfigurations.LstTimer.RemoveAt(JobConfigurations.LstTimer.Count - 1);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });
                    if (cancellectionToken.Token.CanBeCanceled)
                        cancellectionToken.Token.ThrowIfCancellationRequested();
                    Thread.Sleep(50);
                }
            });
        }

        #endregion


        #region Commented code

        #region Random Post Interval Generation

        //private void GenerateRandomIntervals(int maxCount, CancellationTokenSource cancellectionToken)
        //{

        //    Random random = new Random();
        //    var startTime = JobConfigurations.TimeRange.StartTime;
        //    var endTime = JobConfigurations.TimeRange.EndTime;
        //    Task.Factory.StartNew(() =>
        //    {
        //        try
        //        {
        //            for (int noOfPost = 0; noOfPost < maxCount; noOfPost++)
        //            {
        //                cancellectionToken.Token.ThrowIfCancellationRequested();
        //                Application.Current.Dispatcher.Invoke(() =>
        //                {
        //                    JobConfigurations.LstTimer.Add(new TimeSpanHelper() { MidTime = DateTimeUtilities.GetRandomTime(startTime, endTime, random) });
        //                });
        //                Thread.Sleep(10);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    });
        //}

        #endregion

        //private void ChkPostingInterval_OnClick(object sender, RoutedEventArgs e)
        //{

        //    //Task.Factory.StartNew(() =>
        //    //{
        //    //    try
        //    //    {
        //    //        //   CancelToken();
        //    //        Application.Current.Dispatcher.Invoke(() => JobConfigurations.LstTimer.Clear());

        //    //        //  RemoveTimeRange(JobConfigurations.LstTimer.Count, cancellectionToken);
        //    //        cancellectionToken.Token.ThrowIfCancellationRequested();
        //    //        SpecificPostGenerateIntervals(JobConfigurations.MaxPost, cancellectionToken, true);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        ex.DebugLog();
        //    //    }
        //    //});


        //}

        //private void ChkRandomizePublishing_OnClick(object sender, RoutedEventArgs e)
        //{

        //    //Task.Factory.StartNew(() =>
        //    //{
        //    //    try
        //    //    {
        //    //        // CancelToken();
        //    //        Application.Current.Dispatcher.Invoke(() => JobConfigurations.LstTimer.Clear());
        //    //        //  RemoveTimeRange(JobConfigurations.LstTimer.Count, cancellectionToken);
        //    //        cancellectionToken.Token.ThrowIfCancellationRequested();
        //    //        GenerateRandomIntervals(JobConfigurations.MaxPost, cancellectionToken);
        //    //        cancellectionToken.Token.ThrowIfCancellationRequested();
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        ex.DebugLog();
        //    //    }
        //    //});


        //} 

        #endregion
    }
}