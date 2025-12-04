using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using RedditDominatorCore.RDEnums;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using System;

namespace RedditDominatorUI.RDViews.Tools
{
    public class UnSubscriberConfigurationBase : ModuleSettingsUserControl<UnSubscribeViewModel, UnSubscribeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.IsChkCommunitySubscribedBySoftwareChecked ||
                Model.IsChkCommunitySubscribedOutsideSoftwareChecked ||
                Model.IsChkCustomCommunityListChecked) return true;
            Dialog.ShowDialog(this, "Error",
                "Please check atleast one UnSubscribe source option...");
            return false;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (templateModel != null && !string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.UnSubscribeModel =
                        JsonConvert.DeserializeObject<UnSubscribeModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new UnSubscribeViewModel();

                ObjViewModel.UnSubscribeModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for UnSubscriberConfiguration.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class UnSubscriberConfiguration
    {
        public UnSubscriberConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.UnSubscribe,
                Enums.RdMainModule.GrowUnSubscribe.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            VideoTutorialLink = ConstantHelpDetails.UnSubscribeVideoTutorialsLink;
        }

        private static UnSubscriberConfiguration CurrentUnSubscriberConfiguration { get; set; }

        public static UnSubscriberConfiguration GetSingeltonObjectUnSubscriberConfiguration()
        {
            return CurrentUnSubscriberConfiguration ??
                   (CurrentUnSubscriberConfiguration = new UnSubscriberConfiguration());
        }
    }
}