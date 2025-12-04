using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.TwtBlaster;

namespace TwtDominatorUI.TDViews.Tools.Tweet
{
    /// <summary>
    ///     Interaction logic for WelcomeTweetConfiguration.xaml
    /// </summary>
    public class WelcomeTweetConfigurationBase : ModuleSettingsUserControl<WelcomeTweetViewModel, WelcomeTweetModel>
    {
        protected override bool ValidateExtraProperty()
        {
            try
            {
                if (string.IsNullOrEmpty(Model.WelcomeMessageText?.Trim()))
                {
                    Dialog.ShowDialog(this, "Error", "Please enter message");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (templateModel == null)
                {
                    ObjViewModel = new WelcomeTweetViewModel();
                    return;
                }

                if (!string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.WelcomeTweetModel =
                        templateModel.ActivitySettings.GetActivityModel<WelcomeTweetModel>(ObjViewModel.Model, true);

                ObjViewModel.WelcomeTweetModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    public partial class WelcomeTweetConfiguration : WelcomeTweetConfigurationBase
    {
        public WelcomeTweetConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.WelcomeTweet,
                Enums.TdMainModule.TwtBlaster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.WelcomeTweetVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.WelcomeTweetKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}