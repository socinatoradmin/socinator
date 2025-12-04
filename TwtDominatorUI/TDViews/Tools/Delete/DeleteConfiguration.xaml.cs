using System;
using System.Collections.Generic;
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

namespace TwtDominatorUI.TDViews.Tools.Delete
{
    public class DeleteConfigurationBase : ModuleSettingsUserControl<DeleteViewModel, DeleteModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.DeleteSetting.IsChkDeleteTweet && !Model.DeleteSetting.IsChkDeleteComment
                                                      && !Model.DeleteSetting.IsChkUndoRetweet)
            {
                Dialog.ShowDialog(this, "Error",
                    "Please select atleast one Delete source");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.DeleteModel =
                        templateModel.ActivitySettings.GetActivityModel<DeleteModel>(ObjViewModel.Model, true);
                else if (ObjViewModel == null)
                    ObjViewModel = new DeleteViewModel();

                ObjViewModel.DeleteModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DeleteConfiguration.xaml
    /// </summary>
    public partial class DeleteConfiguration : DeleteConfigurationBase
    {
        private List<AccountModel> AccountDetails;


        public ObservableCollectionBase<string> lstAccounts = null;

        public DeleteConfiguration()
        {
            InitializeComponent();


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Delete,
                Enums.TdMainModule.TwtBlaster.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            DialogParticipation.SetRegister(this, this);
            // Help control links. 
            VideoTutorialLink = TDHelpDetails.DeleteVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.DeletetKnowledgeBaseLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }
    }
}