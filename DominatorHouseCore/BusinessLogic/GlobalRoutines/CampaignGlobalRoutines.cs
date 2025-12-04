#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;

#endregion

namespace DominatorHouseCore.BusinessLogic.GlobalRoutines
{
    /// <summary>
    ///     Class which manages all campaigns workflow over the application:
    ///     Start, Update, Delete, Duplicate, Pause, Resume, Stop
    /// </summary>
    public class CampaignGlobalRoutines
    {
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private static readonly CampaignGlobalRoutines _instance = new CampaignGlobalRoutines();
        public static CampaignGlobalRoutines Instance => _instance;

        // UI delegate to select accounts
        public Func<string, bool> ConfirmDialog = msg =>
        {
            //ex.DebugLog();
            return false;
        };

        private readonly IAccountsFileManager _accountsFileManager;


        private CampaignGlobalRoutines()
        {
            _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            _jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
        }


        ///// <summary>
        ///// Creates and saves new template: ActiviySettings as json, activity type 
        ///// </summary>
        ///// <param name="activitySettingsJson"></param>
        ///// <param name="activityType"></param>
        ///// <param name="socialNetworks"></param>
        ///// <param name="templateName"></param>
        ///// <returns></returns>
        //private TemplateModel CreateTempale(string activitySettingsJson, string activityType, SocialNetworks socialNetworks, string templateName)
        //{
        //    // Initialize and assign the values to TemplateModel for store in bin files
        //    TemplateModel newTemplate = new TemplateModel
        //    {
        //        ActivityType = activityType,
        //        ActivitySettings = activitySettingsJson,
        //        CreationDate = DateTime.Now.GetCurrentEpochTime(),
        //        Name = templateName,
        //        SocialNetwork = socialNetworks,
        //    };


        //    // Serialize the template configuration to bin files
        //    InstanceProvider.GetInstance<ITemplatesFileManager>().Add(newTemplate);

        //    return newTemplate;
        //}


        ///// <summary>
        ///// Checks wheter running activities exists for selected users. Asks to overwrite them.
        ///// </summary>
        ///// <param name="activityType"></param>
        ///// <param name="selectedAccounts"></param>
        ///// <returns>false - user choose to stop execution</returns>
        //private bool CheckExistingActivities(ActivityType activityType, List<string> selectedAccounts)
        //{
        //    Debug.Assert(selectedAccounts.Count > 0);

        //    var allAccounts = AccountsFileManager.GetAll();

        //    List<DominatorAccountModel> accountsWithRunningActivity =
        //    allAccounts.Where(
        //        x => _jobActivityConfigurationManager[x.AccountId, activityType]?.TemplateId != null).ToList();

        //    if (accountsWithRunningActivity.Count == 0)
        //        return true;

        //    var selectedAccountsWithRunningActivity = accountsWithRunningActivity.Where(a => selectedAccounts.Contains(a.UserName)).ToList();
        //    if (selectedAccountsWithRunningActivity.Count == 0)
        //        return true;

        //    // Asks for select and overwriting through UI
        //    string accs = string.Join(", ", selectedAccountsWithRunningActivity.Select(a => a.UserName));
        //    string msg = $"{selectedAccountsWithRunningActivity.Count} account(s) already set up to run {activityType} activity." +
        //                  $"Would you like to overwrite settings of those accounts?\r\n{accs}";

        //    if (ConfirmDialog(msg))
        //        return true;

        //    return false;
        //}


        ///// <summary>
        ///// Runs when user clicks Create Campaign
        ///// </summary>
        ///// <param name="newCampaign"></param>
        //public void Create(object activitySettings, ActivityType activityType, string campaignName, List<string> selectedAccounts)
        //{
        //    string activitySettingsJson = Newtonsoft.Json.JsonConvert.SerializeObject(activitySettings);
        //    SocialNetworks socialNetwork = SocinatorInitialize.ActiveSocialNetwork;
        //    TemplateModel template = CreateTempale(activitySettingsJson, activityType.ToString(), socialNetwork, templateName: campaignName);

        //    // Check existing activities and overwrite them if selected account already has running activity with the same type
        //    if (!CheckExistingActivities(activityType, selectedAccounts))
        //        // ReSharper disable once RedundantJumpStatement
        //        return;

        //    // Save 
        //}

        /// <summary>
        ///     Creates and saves new template: ActiviySettings as json, activity type
        /// </summary>
        /// <param name="activitySettingsJson"></param>
        /// <param name="activityType"></param>
        /// <param name="socialNetworks"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        private void CreateTempale(string activitySettingsJson, string activityType, SocialNetworks socialNetworks,
            string templateName)
        {
            // Initialize and assign the values to TemplateModel for store in bin files
            var newTemplate = new TemplateModel
            {
                ActivityType = activityType,
                ActivitySettings = activitySettingsJson,
                CreationDate = DateTime.Now.GetCurrentEpochTime(),
                Name = templateName,
                SocialNetwork = socialNetworks
            };


            // Serialize the template configuration to bin files
            InstanceProvider.GetInstance<ITemplatesFileManager>().Add(newTemplate);
        }


        /// <summary>
        ///     Checks wheter running activities exists for selected users. Asks to overwrite them.
        /// </summary>
        /// <param name="activityType"></param>
        /// <param name="selectedAccounts"></param>
        /// <returns>false - user choose to stop execution</returns>
        private bool CheckExistingActivities(ActivityType activityType, List<string> selectedAccounts)
        {
            Debug.Assert(selectedAccounts.Count > 0);

            var allAccounts = _accountsFileManager.GetAll();

            var accountsWithRunningActivity =
                allAccounts.Where(
                    x => _jobActivityConfigurationManager[x.AccountId, activityType]?.TemplateId != null).ToList();

            if (accountsWithRunningActivity.Count == 0)
                return true;

            var selectedAccountsWithRunningActivity =
                accountsWithRunningActivity.Where(a => selectedAccounts.Contains(a.UserName)).ToList();
            if (selectedAccountsWithRunningActivity.Count == 0)
                return true;

            // Asks for select and overwriting through UI
            var accs = string.Join(", ", selectedAccountsWithRunningActivity.Select(a => a.UserName));
            var msg = string.Format("LangKeyAsksForSelectingAndOverwritingAccounts".FromResourceDictionary(),
                selectedAccountsWithRunningActivity.Count, activityType.ToString(), accs);
            if (ConfirmDialog(msg))
                return true;

            return false;
        }


        /// <summary>
        ///     Runs when user clicks Create Campaign
        /// </summary>
        /// <param name="newCampaign"></param>
        public void Create(object activitySettings, ActivityType activityType, string campaignName,
            List<string> selectedAccounts)
        {
            var activitySettingsJson = JsonConvert.SerializeObject(activitySettings);
            var socialNetwork = SocinatorInitialize.ActiveSocialNetwork;
            CreateTempale(activitySettingsJson, activityType.ToString(), socialNetwork, campaignName);

            // Check existing activities and overwrite them if selected account already has running activity with the same type
            if (!CheckExistingActivities(activityType, selectedAccounts))
                // ReSharper disable once RedundantJumpStatement
                return;

            // Save 
        }
    }
}