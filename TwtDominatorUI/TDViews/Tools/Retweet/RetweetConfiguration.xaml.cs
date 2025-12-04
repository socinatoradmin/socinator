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

namespace TwtDominatorUI.TDViews.Tools.Retweet
{
    public class RetweetConfigurationBase : ModuleSettingsUserControl<RetweetViewModel, RetweetModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.RetweetModel =
                        templateModel.ActivitySettings.GetActivityModel<RetweetModel>(ObjViewModel.Model);
                else if (ObjViewModel == null)
                    ObjViewModel = new RetweetViewModel();

                ObjViewModel.RetweetModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for RetweetConfiguration.xaml
    /// </summary>
    public partial class RetweetConfiguration : RetweetConfigurationBase
    {
        public ObservableCollectionBase<string> lstAccounts = null;

        public RetweetConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Retweet,
                Enums.TdMainModule.TwtBlaster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: RetweetConfigurationSearchControl
            );

            // Help control links. 
            VideoTutorialLink = TDHelpDetails.RetweetVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.RetweetVideoTutorialsLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}