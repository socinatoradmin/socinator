#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IRunningActivityManager
    {
        Task Initialize(IEnumerable<DominatorAccountModel> accountDetails);
        void StartNextRound(DominatorAccountModel accountModel, bool NeedToReSchedule = false);
        void ScheduleIfAccountGotSucess(DominatorAccountModel account);
    }

    public class RunningActivityManager : IRunningActivityManager
    {
        public Task Initialize(IEnumerable<DominatorAccountModel> accountDetails)
        {
            var softwareSettingsFileManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
            var enabledAccount =
                accountDetails.Where(x => x.ActivityManager.LstModuleConfiguration.Any(y => y.IsEnabled));
            if (enabledAccount.Count() > 0)
                if (softwareSettings?.IsEnableParallelActivitiesChecked ?? false)
                    return Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(40));
                        var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                        // everything is allowed
                        foreach (var account in enabledAccount)
                        {
                            dominatorScheduler?.ScheduleEachActivity(account);
                            Task.Delay(2);
                        }
                    });
                else
                    return Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(TimeSpan.FromMinutes(1));
                        // be picky - only one per account (choose wisely)
                        foreach (var account in enabledAccount)
                        {
                            StartNextRound(account);
                            Task.Delay(2);
                        }
                    });

            return Task.CompletedTask;
        }

        public void StartNextRound(DominatorAccountModel accountModel, bool NeedToReSchedule = false)
        {
            var campaignsFileManager =
                InstanceProvider.GetInstance<ICampaignsFileManager>();
            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
            var moduleConfiguration = jobActivityConfigurationManager[accountModel.AccountId].Where(x =>
                    x.IsEnabled && x.NextRun != new DateTime() && (!x.IsTemplateMadeByCampaignMode ||
                                                                   x.IsTemplateMadeByCampaignMode &&
                                                                   campaignsFileManager.Any(
                                                                       y => y.TemplateId ==
                                                                            x.TemplateId /* && y.Status == "Active"*/)))
                .OrderByDescending(PickNextActivity)
                .FirstOrDefault();
            if (moduleConfiguration == null) return;
            //Check if any job process is already scheduled before to run after this activity.
            var schedules = JobManager.AllSchedules;
            var enumerable = schedules as Schedule[] ?? schedules.ToArray();
            var lstOfScheduledJobs =
                enumerable.Where(x => x.Name != null && x.Name.Contains($"{accountModel.AccountId}---"));
            var ofScheduledJobs = lstOfScheduledJobs as Schedule[] ?? lstOfScheduledJobs.ToArray();
            if (ofScheduledJobs.Any())
            {
                var latestScheduledJob = ofScheduledJobs.OrderBy(x => x.NextRun).FirstOrDefault();
                if (latestScheduledJob != null && latestScheduledJob.NextRun < moduleConfiguration.NextRun) return;
                foreach (var scheduledJob in ofScheduledJobs) JobManager.RemoveJob(scheduledJob.Name);
            }

            dominatorScheduler.ScheduleActivityForNextJob(accountModel, moduleConfiguration.ActivityType, NeedToReSchedule);
        }

        public void ScheduleIfAccountGotSucess(DominatorAccountModel account)
        {
            var softwareSettingsFileManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();

            if (account.ActivityManager.LstModuleConfiguration.Any(y => y.IsEnabled))
                if (softwareSettings?.IsEnableParallelActivitiesChecked ?? false)
                {
                    var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
                    dominatorScheduler?.ScheduleEachActivity(account);
                }
                else
                {
                    StartNextRound(account);
                }
        }

        private int PickNextActivity(ModuleConfiguration arg)
        {
            var score = 0; //start from zero
            if (arg.IsEnabled) score += 50;
            var differenceMinutes = DateTime.Now.Subtract(arg.NextRun);
            score += 1 * (int) differenceMinutes.TotalMinutes;
            return score;
        }
    }
}