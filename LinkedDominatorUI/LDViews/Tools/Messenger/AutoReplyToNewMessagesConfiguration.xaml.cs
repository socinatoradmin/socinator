using System;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Messenger;
using LinkedDominatorCore.LDViewModel.Messenger;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Messenger
{
    public class AutoReplyToNewMessagesConfigurationBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel,
        AutoReplyToNewMessageModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.AutoReplyToNewMessageModel =
                        templateModel.ActivitySettings.GetActivityModel<AutoReplyToNewMessageModel>(ObjViewModel.Model,
                            true);
                else
                    ObjViewModel = new AutoReplyToNewMessageViewModel();
                ObjViewModel.AutoReplyToNewMessageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (!ObjViewModel.AutoReplyToNewMessageModel.IsReplyToAllMessagesChecked
                && !ObjViewModel.AutoReplyToNewMessageModel.IsReplyToMessagesThatContainSpecificWord﻿Checked &&
                !ObjViewModel.AutoReplyToNewMessageModel.IsReplyToAllUserMessagesWhodidnotReply
            )

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


            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessagesConfiguration.xaml
    /// </summary>
    public partial class AutoReplyToNewMessagesConfiguration : AutoReplyToNewMessagesConfigurationBase
    {
        public AutoReplyToNewMessagesConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AutoReplyToNewMessage,
                LdMainModules.Messenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AutoReplyToNewMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static AutoReplyToNewMessagesConfiguration CurrentAutoReplyToNewMessagesConfiguration { get; set; }

        public static AutoReplyToNewMessagesConfiguration GetSingeltonObjectAutoReplyToNewMessagesConfiguration()
        {
            return CurrentAutoReplyToNewMessagesConfiguration ?? (CurrentAutoReplyToNewMessagesConfiguration =
                       new AutoReplyToNewMessagesConfiguration());
        }
    }
}