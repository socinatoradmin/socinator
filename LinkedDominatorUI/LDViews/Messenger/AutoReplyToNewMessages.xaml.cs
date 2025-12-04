using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDViewModel.Messenger;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Messenger
{
    public class
        AutoReplyToNewMessageBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel, AutoReplyToNewMessageModel
        >
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (!ObjViewModel.AutoReplyToNewMessageModel.IsReplyToAllMessagesChecked
                && !ObjViewModel.AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked &&
                !ObjViewModel.AutoReplyToNewMessageModel.IsReplyToAllUserMessagesWhodidnotReply)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseSelectAtleastOneOfTheMessageFilters".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked &&
                string.IsNullOrEmpty(ObjViewModel.AutoReplyToNewMessageModel.SpecificWord))
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                    "LangKeyPleaseInputAtleastOneSpecificWord".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Count > 0)
            {
                foreach (var item in ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries)
                    if (item.Content.QueryValue != "All")
                    {
                        string Message = null;
                        try
                        {
                            Message = ObjViewModel.AutoReplyToNewMessageModel.LstDisplayManageMessagesModel
                                .Where(x => x.SelectedQuery.Any(y =>
                                    y.Content.QueryValue.ToString() == item.Content.QueryValue))
                                .Select(x => x.MessagesText).ToList().GetRandomItem();
                        }
                        catch (Exception ex)
                        {
                            if (!ObjViewModel.AutoReplyToNewMessageModel.IsReplyToAllMessagesChecked || !ObjViewModel
                                    .AutoReplyToNewMessageModel.IsReplyToAllUserMessagesWhodidnotReply) Message = "N/A";
                        }

                        if (Message == null)
                        {
                            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                                string.Format("LangKeyPleaseInputAtleastOneMessageForQuery".FromResourceDictionary(),
                                    item.Content.QueryValue));
                            return false;
                        }
                    }

                if (ObjViewModel.AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.Count == 0)
                {
                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                        "LangKeyPleaseInputAtleastOneMessage".FromResourceDictionary());
                    return false;
                }
            }

            return base.ValidateCampaign();
            //return true;
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessage.xaml
    /// </summary>
    public partial class AutoReplyToNewMessage : AutoReplyToNewMessageBase
    {
        private static AutoReplyToNewMessage ObjAutoReplyToNewMessage;

        public AutoReplyToNewMessage()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: AutoReplyToNewMessageFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoReplyToNewMessage,
                moduleName: LdMainModules.Messenger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AutoReplyToNewMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();

            DialogParticipation.SetRegister(this, this);
        }

        public static AutoReplyToNewMessage GetSingeltonAutoReplyToNewMessage()
        {
            return ObjAutoReplyToNewMessage ?? (ObjAutoReplyToNewMessage = new AutoReplyToNewMessage());
        }
    }
}