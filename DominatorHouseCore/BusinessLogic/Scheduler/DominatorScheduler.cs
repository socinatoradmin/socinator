#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using Unity;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IDominatorScheduler
    {
        //void RunActivity(DominatorAccountModel account, string templateId, TimingRange currentJobTimeRange,
        //    string module);
        Task RunActivity(DominatorAccountModel account, string templateId, TimingRange currentJobTimeRange,
            bool shouldStop, DateTime stopTime,
            string module);

        bool Stop(string accountName, string templateId, bool isStopIfAccountLoginFail = false);

        void StopActivity(DominatorAccountModel account, string module, string templateId, bool needRestart,
            bool isTimelimitReached = false);

        bool CompareRunningTime(List<RunningTimes> firstRunningTime, List<RunningTimes> secondRunningTime);
        bool ChangeAccountsRunningStatus(bool isStart, string accountId, ActivityType activityType);
        bool EnableDisableModules(ActivityType stopActivity, ActivityType startActivity, string accountId);
        void ScheduleEachActivity(DominatorAccountModel account);
        void ScheduleActivityForNextJob(DominatorAccountModel dominatorAccount, ActivityType activityType, bool NeedToReSchedule = false);
        void ScheduleNextActivity(DominatorAccountModel dominatorAccountModel, ActivityType activityType, bool NeedToReSchedule = false);

        void RescheduleifLimitReached(IJobProcess jobProcess, ReachedLimitInfo limitInfo, ReachedLimitType limitType
            , int scheduleAfterXXHours = 0);
    }

    public class DominatorScheduler : IDominatorScheduler
    {
        public object RunStopActivityLocker = new object();
        private readonly IRunningActivityManager _runningActivityManager;
        private readonly ISchedulerProxy _schedulerProxy;
        private readonly IJobLimitsHolder _jobLimitsHolder;
        private readonly IAccountsCacheService _accountsCacheService;
        private readonly IJobCountersManager _jobCountersManager;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IRunningJobsHolder _runningJobsHolder;
        private readonly IJobProcessScopeFactory _jobProcessScopeFactory;
        public static SemaphoreSlim _lockWithThreadLimit;
        public static SemaphoreSlim _lockWithThreadLimitAsPerConfig;
        public static int maxThreadCount;
        public static int maxBrowserInstances;
        public static bool islogged;

        public DominatorScheduler(IRunningActivityManager runningActivityManager, ISchedulerProxy schedulerProxy,
            IJobLimitsHolder jobLimitsHolder, IJobProcessScopeFactory jobProcessScopeFactory,
            IAccountsCacheService accountsCacheService, IJobCountersManager jobCountersManager,
            IJobActivityConfigurationManager jobActivityConfigurationManager, IRunningJobsHolder runningJobsHolder)
        {
            _runningActivityManager = runningActivityManager;
            _schedulerProxy = schedulerProxy;
            _jobLimitsHolder = jobLimitsHolder;
            _jobProcessScopeFactory = jobProcessScopeFactory;
            _accountsCacheService = accountsCacheService;
            _jobCountersManager = jobCountersManager;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _runningJobsHolder = runningJobsHolder;
            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettings>();
            if (softwareSettings.Settings?.IsThreadLimitChecked ?? false)
            {
                islogged = false;
                maxThreadCount = softwareSettings.Settings.MaxThreadCount;
                _lockWithThreadLimit = new SemaphoreSlim(maxThreadCount, maxThreadCount);
            }

            SetThreadCountAsPerConfig();
        }

        /// <summary>
        ///     To start the activity of template for the given account at specified time range
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateId"></param>
        /// <param name="currentJobTimeRange"></param>
        /// <param name="module"></param>
        //public void RunActivity(DominatorAccountModel account, string templateId, TimingRange currentJobTimeRange, string module)
        //{
        //    try
        //    {
        //        lock (RunStopActivityLocker)
        //        {
        //            var id = JobProcess.AsId(account.AccountBaseModel.AccountId, templateId);

        //            var scheduledJob = _schedulerProxy[id];

        //            if (scheduledJob != null && scheduledJob.Disabled)
        //                return;

        //            var scope = _jobProcessScopeFactory.GetScope(account,
        //                           (ActivityType)Enum.Parse(typeof(ActivityType), module), templateId, currentJobTimeRange,
        //                           account.AccountBaseModel.AccountNetwork);
        //            var activeJobProcessFactory =
        //                scope.Resolve<IJobProcessFactory>(account.AccountBaseModel.AccountNetwork
        //                    .ToString());
        //            var jobProcess = activeJobProcessFactory.Create(account.AccountBaseModel.UserName, templateId,
        //                currentJobTimeRange, module, account.AccountBaseModel.AccountNetwork);
        //            _jobLimitsHolder.Reset(jobProcess.Id, jobProcess.JobConfiguration);
        //            var limitInfo = jobProcess.CheckLimit();
        //            if (limitInfo.ReachedLimitType != ReachedLimitType.NoLimit)
        //            {
        //                RescheduleifLimitReached(jobProcess, limitInfo, limitInfo.ReachedLimitType);
        //                return;
        //            }

        //            jobProcess.StartProcessAsync().ContinueWith(a => scope.Dispose());
        //            jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}
        private static bool needTolock = true;

        public async Task RunActivity(DominatorAccountModel account, string templateId, TimingRange currentJobTimeRange,
            bool shouldStop, DateTime stopTime, string module)
        {
            if (_lockWithThreadLimit != null && needTolock)
            {
                if (_lockWithThreadLimit?.CurrentCount == 0 && !islogged)
                {
                    islogged = true;
                    GlobusLogHelper.log.Info(
                        $"{"LangKeyThreadLimitReachedTo".FromResourceDictionary()} {maxThreadCount} {"LangKeyPendingStartsWhenRunnningStops".FromResourceDictionary()}");
                }

                _lockWithThreadLimit?.Wait();
            }

            if (account.IsRunProcessThroughBrowser && _lockWithThreadLimitAsPerConfig != null)
                _lockWithThreadLimitAsPerConfig?.Wait();


            if (!account.ActivityManager.LstModuleConfiguration.Any(y =>
                y.IsEnabled && y.ActivityType.ToString() == module))
            {
                if (_lockWithThreadLimit?.CurrentCount != maxThreadCount)
                    _lockWithThreadLimit?.Release();

                if (account.IsRunProcessThroughBrowser && _lockWithThreadLimitAsPerConfig != null)
                {
                    if (_lockWithThreadLimitAsPerConfig?.CurrentCount != maxBrowserInstances)
                        _lockWithThreadLimitAsPerConfig?.Release();
                }
                needTolock = false;
                return;
            }

            needTolock = true;
            try
            {
                var id = JobProcess.AsId(account.AccountBaseModel.AccountId, templateId);

                var scheduledJob = _schedulerProxy[id];

                if (scheduledJob != null && scheduledJob.Disabled)
                    return;

                var scope = _jobProcessScopeFactory.GetScope(account,
                    (ActivityType)Enum.Parse(typeof(ActivityType), module), templateId, currentJobTimeRange,
                    account.AccountBaseModel.AccountNetwork);
                var activeJobProcessFactory =
                    scope.Resolve<IJobProcessFactory>(account.AccountBaseModel.AccountNetwork
                        .ToString());
                var jobProcess = activeJobProcessFactory.Create(account.AccountBaseModel.UserName, templateId,
                    currentJobTimeRange, module, account.AccountBaseModel.AccountNetwork);
                _jobLimitsHolder.Reset(jobProcess.Id, jobProcess.JobConfiguration);
                var limitInfo = jobProcess.CheckLimit();
                if (limitInfo.ReachedLimitType != ReachedLimitType.NoLimit)
                {
                    RescheduleifLimitReached(jobProcess, limitInfo, limitInfo.ReachedLimitType);
                    return;
                }

                if (shouldStop && DateTime.Now >= stopTime)
                {
                    if (_lockWithThreadLimit?.CurrentCount != maxThreadCount)
                        _lockWithThreadLimit?.Release();

                    if (account.IsRunProcessThroughBrowser &&
                        _lockWithThreadLimitAsPerConfig?.CurrentCount != maxBrowserInstances)
                        _lockWithThreadLimitAsPerConfig?.Release();

                    return;
                }

                await jobProcess.StartProcessAsync().ContinueWith(a => scope.Dispose());
                jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                if (_lockWithThreadLimit?.CurrentCount != maxThreadCount)
                    _lockWithThreadLimit?.Release();

                if (account.IsRunProcessThroughBrowser &&
                    _lockWithThreadLimitAsPerConfig?.CurrentCount != maxBrowserInstances)
                    _lockWithThreadLimitAsPerConfig?.Release();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void StopActivity(DominatorAccountModel account, string module,
            string templateId, bool needRestart, bool isTimelimitReached = false)
        {
            lock (RunStopActivityLocker)
            {
                Stop(account.AccountId, templateId);
                try
                {
                    var moduleConfiguration =
                        _jobActivityConfigurationManager[account.AccountId]
                            .FirstOrDefault(x => x.TemplateId == templateId);
                    moduleConfiguration = moduleConfiguration ??
                                          _jobActivityConfigurationManager[account.AccountId]
                                              .FirstOrDefault(x => x.ActivityType.ToString() == module);
                    if (moduleConfiguration != null)
                    {
                        if (isTimelimitReached)
                            moduleConfiguration.NextRun = DateTimeUtilities.GetNextStartTime(moduleConfiguration);

                        moduleConfiguration.IsEnabled = needRestart;
                        if (moduleConfiguration.Status == null ||
                            moduleConfiguration.Status == "Active" && !moduleConfiguration.IsEnabled ||
                            moduleConfiguration.Status == "Paused" && moduleConfiguration.IsEnabled)
                            moduleConfiguration.Status = moduleConfiguration.IsEnabled ? "Active" : "Paused";
                        _jobActivityConfigurationManager.AddOrUpdate(account.AccountId,
                            moduleConfiguration.ActivityType, moduleConfiguration);
                        _accountsCacheService.UpsertAccounts(account);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                var id = JobProcess.AsId(account.AccountId, templateId);
                _schedulerProxy.RemoveJob(id);
                var scheduledJob = _schedulerProxy[id];
                try
                {
                    if (!needRestart)
                    {
                        //if activity of account is stopped then check if any other activity is enable with that account
                        //if enabled then start next round with enabled activity
                        _runningActivityManager.StartNextRound(account);
                        return;
                    }

                    if (scheduledJob == null)
                    {
                        ScheduleNextActivity(account, (ActivityType)Enum.Parse(typeof(ActivityType), module),true);
                        return;
                    }

                    scheduledJob.Disable();

                    if (!scheduledJob.Disabled)
                        ScheduleNextActivity(account, (ActivityType)Enum.Parse(typeof(ActivityType), module),true);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }


        public bool CompareRunningTime(List<RunningTimes> firstRunningTime, List<RunningTimes> secondRunningTime)
        {
            if (firstRunningTime == null && secondRunningTime != null ||
                secondRunningTime == null && firstRunningTime != null)
                return false;
            if (firstRunningTime == null)
                return true;

            if (firstRunningTime.Count != secondRunningTime.Count)
                return false;

            var IsEqual = true;
            foreach (var item in firstRunningTime)
            {
                var oldRunningTime = item;
                var newRunningTime = secondRunningTime.ElementAt(firstRunningTime.IndexOf(item));


                if (oldRunningTime == null && newRunningTime != null ||
                    newRunningTime == null && oldRunningTime != null)
                {
                    IsEqual = false;
                    break;
                }

                if (oldRunningTime.Timings.Count != newRunningTime.Timings.Count)
                    return false;
                foreach (var oldtime in oldRunningTime.Timings)
                {
                    if (!newRunningTime.Timings.Contains(oldtime))
                        return false;
                    // ReSharper disable once RedundantAssignment
                    IsEqual = true;
                }
            }

            return IsEqual;
        }

        public bool ChangeAccountsRunningStatus(bool isStart, string accountId, ActivityType activityType)
        {
            var campaignFileManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
            var accountModel = _accountsCacheService[accountId];
            var moduleConfiguration = _jobActivityConfigurationManager[accountModel?.AccountId, activityType];
            if (moduleConfiguration == null)
                return false;
            try
            {
                var accountstemplateId = moduleConfiguration.TemplateId;
                if (isStart)
                {
                    try
                    {
                        var campaignStatus = campaignFileManager
                            .FirstOrDefault(x => x.TemplateId == moduleConfiguration.TemplateId)
                            ?.Status;
                        if (campaignStatus == "Paused" && moduleConfiguration.IsEnabled)
                        {
                            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                                "LangKeyErrorCampaignConfigurationIsPaused".FromResourceDictionary());
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    moduleConfiguration.IsEnabled = true;
                    ScheduleNextActivity(accountModel, activityType);
                }
                else
                {
                    moduleConfiguration.IsEnabled = false;
                    StopActivity(accountModel, activityType.ToString(), accountstemplateId, false);
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                _jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountBaseModel.AccountId, activityType,
                    moduleConfiguration);
                _accountsCacheService.UpsertAccounts(accountModel);
            }
        }

        /// <summary>
        ///     This method can be used in cases like Enable AutoFollow/Unfollow. You need to pass the Activity type which has to
        ///     be disabled as first parameter and ActivityType which has to be enabled as second parameter, and accountID for
        ///     which the activities has to be updated.
        /// </summary>
        /// <param name="stopActivity">ActivityType which has to be disabled</param>
        /// <param name="startActivity">ActivityType which has to be enabled</param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool EnableDisableModules(ActivityType stopActivity, ActivityType startActivity, string accountId)
        {
            try
            {
                if (ChangeAccountsRunningStatus(false, accountId, stopActivity))
                    if (ChangeAccountsRunningStatus(true, accountId, startActivity))
                        return true;
                    else
                        throw new InvalidOperationException(
                            $"Error Code : 1001 Cant able start the activity {startActivity} with account {accountId}");
                throw new InvalidOperationException(
                    $"Error Code : 1002  Cant able stop the activity {stopActivity} with account {accountId}");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public void ScheduleEachActivity(DominatorAccountModel account)
        {
            try
            {
                foreach (var moduleConfiguration in _jobActivityConfigurationManager[account.AccountId])
                    ScheduleNextActivity(account, moduleConfiguration.ActivityType);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ScheduleActivityForNextJob(DominatorAccountModel dominatorAccount, ActivityType activityType,bool NeedToReSchedule = false)
        {
            var moduleConfiguration = _jobActivityConfigurationManager[dominatorAccount.AccountId, activityType];
            if (moduleConfiguration == null || !moduleConfiguration.IsEnabled)
                return;

            // Check if activity with the same id already running
            if (_runningJobsHolder.IsRunning(new JobKey(dominatorAccount.AccountId, moduleConfiguration.TemplateId)))
                //GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.UserName, activityType,$"User {dominatorAccount.UserName} is already running with {activityType} activity");
                return;

            var softwareSettings = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            if (!softwareSettings.GetSoftwareSettings().IsEnableParallelActivitiesChecked
                && _runningJobsHolder.IsActivityRunningForAccount(dominatorAccount.AccountId))
                return;

            try
            {
                //Get the time when the activity has to be performed next and stopped.
                var timeToRunNext = NeedToReSchedule ?
                    DateTimeUtilities.GetNextStartTime(moduleConfiguration, ReachedLimitType.Job,moduleConfiguration?.DelayBetweenJobs?.GetRandom())
                    :DateTimeUtilities.GetNextStartTime(moduleConfiguration, ReachedLimitType.Job);
                if (timeToRunNext == DateTime.MinValue)
                {
                    GlobusLogHelper.log.Debug(
                        $"suspicious calculation {timeToRunNext} - {DateTime.Now.AddMinutes(-1)}");
                    return;
                }

                if (timeToRunNext < moduleConfiguration.NextRun) timeToRunNext = moduleConfiguration.NextRun;
                var stopTime = timeToRunNext.Date;
                var templateId = moduleConfiguration.TemplateId;
                var jobId = JobProcess.AsId(dominatorAccount.AccountId, templateId);

                var index = (int)timeToRunNext.DayOfWeek;

                var timings = new List<TimingRange>();
                var time = new TimingRange(TimeSpan.MinValue, TimeSpan.MaxValue)
                {
                    Module = activityType.ToString()
                };

                #region Schedulling logic

                int? nextIndex = null;
                List<TimingRange> nextTimings = null;
                var shouldStop = false;
                var setTime = false;
                var resetTime = false;
                // this loop is for 7 days of the week
                for (var i = 0; i < 7; i++)
                {
                    var daysGap = 1;
                    // at first iteration nextindex will null because it's getting initialized below
                    if (nextIndex == null)
                    {
                        index += i;

                        while (true)
                        {
                            if (index > 6
                            ) // A week has only 7 days, so if index gets the value more than 6, we are making it 0 as sunday.
                                index = 0;

                            // get running times for todays or account scheduling date
                            var runningTimes = moduleConfiguration.LstRunningTimes[index];
                            if (runningTimes.Timings.Count == 0)
                            {
                                resetTime = true;
                                i++;
                                index++;
                                continue;
                            }

                            if (resetTime)
                                timeToRunNext = timeToRunNext.Date.AddDays(i)
                                    .Add(runningTimes.Timings.First().StartTime);

                            timings = runningTimes.Timings.ToList();
                            timings.Sort(new RunningTimeComparer());
                            break;
                        }
                    }
                    else if (nextTimings.Count > 0)
                    {
                        // after first iteration we are intializing last index and timing to current index and timings
                        index = nextIndex ?? 0;
                        timings = nextTimings;
                    }

                    var addDayToStop = i;
                    while (true)
                    {
                        nextIndex = index + 1;
                        if (nextIndex > 6
                        ) // A week has only 7 days, so if index gets the value more than 6, we are making it 0 as sunday.
                            nextIndex = 0;
                        // get running times for next days of todays or account scheduling date
                        var nextRunningTimes = moduleConfiguration.LstRunningTimes[nextIndex ?? 0];

                        if (nextRunningTimes.Timings.Count == 0)
                        {
                            i++;
                            daysGap++;
                            index = nextIndex ?? 0;
                            continue;
                        }

                        nextTimings = nextRunningTimes.Timings.ToList();
                        nextTimings.Sort(new RunningTimeComparer());
                        break;
                    }

                    //sorted timings of the day selected
                    foreach (var timing in timings)
                    {
                        // getting a runnig time which should be between the startTime and endTime
                        if (!setTime && timing.StartTime <= timeToRunNext.TimeOfDay &&
                            timeToRunNext.TimeOfDay < timing.EndTime)
                        {
                            time = timing;
                            setTime = true;
                            if (daysGap > 1)
                            {
                                stopTime = timeToRunNext.Date.Add(timing.EndTime);
                                shouldStop = true;
                                break;
                            }
                        }

                        // getting a stop time whose end time should not be as 23:59:59 and endTime as 0:0:0 togather
                        if (daysGap > 1 ||
                            timing.EndTime == new TimeSpan(23, 59, 59) &&
                            nextTimings.First().StartTime > new TimeSpan(0, 0, 0) ||
                            timing.EndTime < new TimeSpan(23, 59, 59) &&
                            nextTimings.First().StartTime == new TimeSpan(0, 0, 0) ||
                            timing.EndTime < new TimeSpan(23, 59, 59) &&
                            nextTimings.First().StartTime > new TimeSpan(0, 0, 0))
                        {
                            stopTime = stopTime.Date.AddDays(addDayToStop).Add(timing.EndTime);
                            shouldStop = true;
                            break;
                        }
                    }

                    if (shouldStop)
                        break;
                }

                #endregion

                if (timeToRunNext >= stopTime)
                    stopTime = stopTime.Date.AddDays(1);

                if (timeToRunNext < DateTime.Now) timeToRunNext = timeToRunNext.AddSeconds(25);

                if (moduleConfiguration.DelayBetweenAccounts.GetRandom() > 0)
                {
                    var delay = moduleConfiguration.AccountCount * moduleConfiguration.DelayBetweenAccounts.GetRandom();
                    timeToRunNext = timeToRunNext.AddSeconds(delay);
                }

                UpdatedScheduleJob(dominatorAccount, time, templateId, jobId, timeToRunNext, stopTime, shouldStop);
                GlobusLogHelper.log.Info(Log.NextJobExpectedToStartBy,
                    dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.AccountBaseModel.UserName,
                    activityType, timeToRunNext);
            }
            catch (InvalidOperationException)
            {
                ChangeAccountsRunningStatus(false, dominatorAccount.AccountId, activityType);
                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                    dominatorAccount.UserName, activityType,
                    string.Format("LangKeyErrorActivityIsntConfiguredProperlyForAccount".FromResourceDictionary(),
                        activityType));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ScheduleNextActivity(DominatorAccountModel dominatorAccountModel, ActivityType activityType, bool NeedToReSchedule = false)
        {
            var softwareSettingsFileManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();

            if (softwareSettings?.IsEnableParallelActivitiesChecked ?? false)
            {
                ScheduleActivityForNextJob(dominatorAccountModel, activityType);
            }
            else
            {
                var moduleConfiguration = _jobActivityConfigurationManager[dominatorAccountModel.AccountId]
                    .Where(x => x.IsEnabled);

                foreach (var config in moduleConfiguration)
                {
                    var id = JobProcess.AsId(dominatorAccountModel.AccountId, config.TemplateId);
                    if (_runningJobsHolder.IsRunning(id))
                        return;
                }

                _runningActivityManager.StartNextRound(dominatorAccountModel, NeedToReSchedule);
            }
        }

        public bool Stop(string accountName, string templateId, bool isStopIfAccountLoginFail = false)
        {
            try
            {
                var id = new JobKey(accountName, templateId);

                if (!_runningJobsHolder.Stop(id, isStopIfAccountLoginFail))
                {
                    GlobusLogHelper.log.Trace($"Job process with Id - {id} not found");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
                return false;
            }
        }

        private void UpdatedScheduleJob(DominatorAccountModel dominatorAccount, TimingRange timing, string templateId,
            string jobId, DateTime timeToRunNext, DateTime stopTime, bool shouldStop)
        {
            _schedulerProxy.AddJob(async () =>
            {
                try
                {
                    await RunActivity(dominatorAccount, templateId, timing, shouldStop, stopTime, timing.Module);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }, s => s.WithName(jobId).ToRunOnceAt(timeToRunNext));

            if (shouldStop)
                _schedulerProxy.AddJob(() =>
                {
                    try
                    {
                        StopActivity(dominatorAccount, timing.Module, templateId, true, true);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }, s => s.ToRunOnceAt(stopTime));
        }

        public void RescheduleifLimitReached(IJobProcess jobProcess, ReachedLimitInfo limitInfo,
            ReachedLimitType limitType, int scheduleAfterXXHours = 0)
        {
            //jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            var customTimeToSchedule = DateTime.Now.AddHours(scheduleAfterXXHours);

            // For enabling job after n hours
            if (limitInfo.LimitValue > 0)
                GlobusLogHelper.log.Info(limitInfo.ReachedLimitType.ConvertToLogRecord(),
                    jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    jobProcess.DominatorAccountModel.AccountBaseModel.UserName, jobProcess.ActivityType,
                    limitInfo.LimitValue);

            Stop(jobProcess.DominatorAccountModel.AccountId, jobProcess.TemplateId);
            //here jobProcess.JobCancellationTokenSource.Token become true because campaign is stopped here

            var moduleConfiguration =
                _jobActivityConfigurationManager[jobProcess.DominatorAccountModel.AccountId, jobProcess.ActivityType];
            var nextStartTime = limitType == ReachedLimitType.Job
                ? DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType,
                    jobProcess.JobConfiguration.DelayBetweenJobs.GetRandom())
                : DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType);

            if (scheduleAfterXXHours > 0 && DateTime.Compare(customTimeToSchedule, nextStartTime) > 0)
                nextStartTime = customTimeToSchedule;

            if (moduleConfiguration != null)
            {
                moduleConfiguration.NextRun = nextStartTime;
                moduleConfiguration.IsEnabled = true;
                _jobActivityConfigurationManager.AddOrUpdate(
                    jobProcess.DominatorAccountModel.AccountBaseModel.AccountId, jobProcess.ActivityType,
                    moduleConfiguration);
                _accountsCacheService.UpsertAccounts(jobProcess.DominatorAccountModel);
            }

            StopActivity(jobProcess.DominatorAccountModel, jobProcess.ActivityType.ToString(), jobProcess.TemplateId,
                moduleConfiguration.IsEnabled);
            _jobCountersManager.Reset(jobProcess.Id);
            //jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
        }

        private void SetThreadCountAsPerConfig()
        {
            try
            {
                if (_lockWithThreadLimitAsPerConfig == null)
                {
                    var perfCounterService = InstanceProvider.GetInstance<IPerfCounterService>();
                    var ramSize = Convert.ToInt32(perfCounterService.LoadedMemoryDescrption.Replace(" MB", ""));
                    if (ramSize < 5000)
                    {
                        maxBrowserInstances = 6;
                        _lockWithThreadLimitAsPerConfig = new SemaphoreSlim(maxBrowserInstances, maxBrowserInstances);
                    }
                    else if (ramSize < 9000)
                    {
                        maxBrowserInstances = 10;
                        _lockWithThreadLimitAsPerConfig = new SemaphoreSlim(maxBrowserInstances, maxBrowserInstances);
                    }
                    else
                    {
                        maxBrowserInstances = 15;
                        _lockWithThreadLimitAsPerConfig = new SemaphoreSlim(maxBrowserInstances, maxBrowserInstances);
                    }

                }

            }
            catch (Exception)
            {
                if (_lockWithThreadLimitAsPerConfig == null)
                {
                    maxBrowserInstances = 10;
                    _lockWithThreadLimitAsPerConfig = new SemaphoreSlim(maxBrowserInstances, maxBrowserInstances);
                }
            }
        }

    }
}