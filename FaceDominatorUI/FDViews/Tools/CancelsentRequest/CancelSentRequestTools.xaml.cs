using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDViewModel.FriendsViewModel;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.CancelsentRequest
{
    public class
        CancelSentRequestToolsBase : ModuleSettingsUserControl<CancenSentRequestViewModel, CancelSentRequestModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.UnfriendOptionModel.IsAddedThroughSoftware && !Model.UnfriendOptionModel.IsAddedOutsideSoftware)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyselectAtLeastOneSource".FromResourceDictionary());
                return false;
            }

            if (Model.UnfriendOptionModel.IsFilterApplied &&
                (Model.UnfriendOptionModel.DaysBefore == 0 && Model.UnfriendOptionModel.HoursBefore == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectValidSourceFilter".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CancelSentRequestModel =
                        templateModel.ActivitySettings.GetActivityModelNonQueryList<CancelSentRequestModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new CancenSentRequestViewModel();
                ObjViewModel.CancelSentRequestModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for CancelSentRequestTools.xaml
    /// </summary>
    public partial class CancelSentRequestTools
    {
        public CancelSentRequestTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.WithdrawSentRequest,
                FdMainModule.Friends.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.WithDrawVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.WithDrawKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static CancelSentRequestTools CurrentCancelSentRequestTools { get; set; }

        public static CancelSentRequestTools GetSingeltonObjectCancelSentRequestTools()
        {
            return CurrentCancelSentRequestTools ?? (CurrentCancelSentRequestTools = new CancelSentRequestTools());
        }
    }
}