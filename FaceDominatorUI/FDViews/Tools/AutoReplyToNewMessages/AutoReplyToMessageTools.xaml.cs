using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.AutoReplyToNewMessages
{
    public class
        AutoReplyToMessageToolsBase : ModuleSettingsUserControl<AutoReplyMessageViewModel, AutoReplyMessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!Model.AutoReplyOptionModel.IsFriendsMessageChecked &&
                !Model.AutoReplyOptionModel.IsMessageRequestChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyselectAtLeastOneSource".FromResourceDictionary());
                return false;
            }

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
                return false;
            }

            if (Model.AutoReplyOptionModel.IsFilterApplied &&
                (Model.AutoReplyOptionModel.DaysBefore == 0 && Model.AutoReplyOptionModel.HoursBefore == 0))
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
                    ObjViewModel.AutoReplyMessageModel =
                        templateModel.ActivitySettings.GetActivityModelNonQueryList<AutoReplyMessageModel>(ObjViewModel
                            .Model);
                else
                    ObjViewModel = new AutoReplyMessageViewModel();
                ObjViewModel.AutoReplyMessageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToMessageTools.xaml
    /// </summary>
    public partial class AutoReplyToMessageTools
    {
        /*
                QueryContent _queryContent = new QueryContent { Content = new QueryInfo { QueryValue = "All" } };
        */

        public AutoReplyToMessageTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AutoReplyToNewMessage,
                FdMainModule.Messanger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.ReplyMessageVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.ReplyMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static AutoReplyToMessageTools CurrentGroupJoinerTools { get; set; }

        public static AutoReplyToMessageTools GetSingeltonObjectGroupJoinerTools()
        {
            return CurrentGroupJoinerTools ?? (CurrentGroupJoinerTools = new AutoReplyToMessageTools());
        }
    }
}