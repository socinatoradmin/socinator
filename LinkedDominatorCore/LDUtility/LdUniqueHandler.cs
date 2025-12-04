using System;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace LinkedDominatorCore.LDUtility
{
    public interface ILdUniqueHandler
    {
        bool IsCampaignWiseUnique(UniquePreRequisticProperties unique);
        bool IsGlobalUnique(UniquePreRequisticProperties unique);
    }

    public class LdUniqueHandler : ILdUniqueHandler
    {
        public bool IsCampaignWiseUnique(UniquePreRequisticProperties unique)
        {
            #region Unique Activity When Running Multiple Account

            var jobActivityConfigurationManager =
                InstanceProvider.GetInstance<IJobActivityConfigurationManager>();
            var moduleSetting = jobActivityConfigurationManager[unique.AccountModel.AccountId, unique.ActivityType];
            var ldCampaignInteractionDetails = InstanceProvider.GetInstance<ICampaignInteractionDetails>();

            if (moduleSetting != null && unique.IsUniqueOperationChecked && moduleSetting.IsTemplateMadeByCampaignMode)
                try
                {
                    var profileUrl = unique.ProfileUrl;
                    if (unique.ProfileUrl.Contains("sales/"))
                        profileUrl = Utilities.GetBetween("$$" + unique.ProfileUrl, "$$", ",");
                    ldCampaignInteractionDetails.AddInteractedData(SocialNetworks.LinkedIn, unique.CampaignId,
                        profileUrl);
                }
                catch (Exception)
                {
                    return true;
                }

            #endregion

            return false;
        }

        public bool IsGlobalUnique(UniquePreRequisticProperties unique)
        {
            try
            {
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var linkedinConfig =
                    genericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile());
                var ldGlobalInteractionDetails = InstanceProvider.GetInstance<IGlobalInteractionDetails>();

                if (linkedinConfig.IsEnableSendConnectionRequestToDifferentUsers)
                {
                    #region Check in LDGlobalInteractionDetails

                    // in sales connection request user we having different urls hence we splitting it.
                    var profileUrl = unique.ProfileUrl;
                    if (unique.ProfileUrl.Contains("sales/"))
                        profileUrl = Utilities.GetBetween("$$" + unique.ProfileUrl, "$$", ",");
                    ldGlobalInteractionDetails.AddInteractedData(SocialNetworks.LinkedIn, unique.ActivityType,
                        profileUrl);

                    #endregion
                }
            }
            catch (Exception)
            {
                return true;
            }

            return false;
        }
    }

    public class UniquePreRequisticProperties
    {
        public DominatorAccountModel AccountModel { get; set; }
        public bool IsUniqueOperationChecked { get; set; }
        public string ProfileUrl { get; set; }
        public ActivityType ActivityType { get; set; }
        public string CampaignId { get; set; }
    }
}