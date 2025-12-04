using System.Collections.Generic;
using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup
{
    public interface ISaveSetting
    {
        void Save();
    }

    public class SaveSetting : ISaveSetting
    {
        private IRegionManager _regionManager;
        private readonly ISelectActivityViewModel _selectActivityViewModel;

        public SaveSetting(ISelectActivityViewModel selectActivityViewModel, IRegionManager region)
        {
            _regionManager = region;
            _selectActivityViewModel = selectActivityViewModel;
        }

        public void Save()
        {
            var account = _selectActivityViewModel.SelectAccount;
            var dominatorScheduler = InstanceProvider.GetInstance<IDominatorScheduler>();
            StartupBaseViewModel.ViewModelToSave.ForEach(data =>
            {
                var templateId = TemplateModel.SaveTemplate(data.Model, data.ActivityType.ToString(),
                    account.AccountBaseModel.AccountNetwork,
                    string.Empty);
                SaveTemplateToAccounts(templateId, account, data.Model, data.ActivityType);
                dominatorScheduler.ScheduleNextActivity(account, data.ActivityType);
            });
        }

        private void SaveTemplateToAccounts(string templateId, DominatorAccountModel account, dynamic Model,
            ActivityType _activityType)
        {
            var _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            List<RunningTimes> runningTime = Model.JobConfiguration.RunningTime;
            var accountDetails = _accountsFileManager.GetAccountById(account.AccountBaseModel.AccountId);
            AddTemplateToAccount(templateId, accountDetails, runningTime, _activityType, Model);

            var accountsCacheService = InstanceProvider.GetInstance<IAccountsCacheService>();
            accountsCacheService.UpsertAccounts(accountDetails);
        }

        private void AddTemplateToAccount(string templateId, DominatorAccountModel account,
            List<RunningTimes> runningTime, ActivityType _activityType, dynamic Model)
        {
            var _jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                _jobActivityConfigurationManager[account.AccountBaseModel.AccountId, _activityType] ??
                new ModuleConfiguration { ActivityType = _activityType };

            moduleConfiguration.LastUpdatedDate = DateTimeUtilities.GetEpochTime();
            moduleConfiguration.IsEnabled = true;
            moduleConfiguration.Status = "Active";
            moduleConfiguration.TemplateId = templateId;
            moduleConfiguration.IsTemplateMadeByCampaignMode = true;
            moduleConfiguration.DelayBetweenJobs = Model.JobConfiguration.DelayBetweenJobs;
            moduleConfiguration.DelayBetweenAccounts = Model.JobConfiguration.DelayBetweenAccounts;
            runningTime.ForEach(x =>
            {
                foreach (var timingRange in x.Timings) timingRange.Module = _activityType.ToString();
            });
            moduleConfiguration.LstRunningTimes = new List<RunningTimes>(runningTime);

            moduleConfiguration.NextRun = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration, 0);
            _jobActivityConfigurationManager.AddOrUpdate(account.AccountBaseModel.AccountId,
                moduleConfiguration.ActivityType, moduleConfiguration);
            var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            //Update ActivityManager of account in Db
            globalDbOperation.UpdateAccountActivityManager(account);
        }
    }
}