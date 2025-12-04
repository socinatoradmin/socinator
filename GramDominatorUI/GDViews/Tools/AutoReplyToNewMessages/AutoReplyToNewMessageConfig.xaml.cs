using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.Instachat;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.AutoReplyToNewMessages
{
    public class
        AutoReplyToNewMessageConfigBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel,
            AutoReplyToNewMessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check Specific words provided or not
            if (Model.IsReplyToMessagesThatContainSpecificWord﻿Checked &&
                string.IsNullOrEmpty(Model.SpecificWord.Trim()))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add specific word(s) for message filter.");
                return false;
            }

            // Check message(s) has provided or not
            if (Model.IsReplyToMessagesThatContainSpecificWord﻿Checked &&
                string.IsNullOrEmpty(ObjViewModel.AutoReplyToNewMessageModel.SpecificWord))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "please input at least one specific word");
                return false;
            }

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error", "Please provide message(s)");
                return false;
            }

            if (ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Count > 0)
                foreach (var item in Model.ManageMessagesModel.LstQueries)
                    if (item.Content.QueryValue != "All")
                    {
                        string message = null;
                        try
                        {
                            message = ObjViewModel.AutoReplyToNewMessageModel.LstDisplayManageMessageModel
                                .Where(x => x.SelectedQuery.Any(y =>
                                    y.Content.QueryValue.ToString() == item.Content.QueryValue))
                                .Select(x => x.MessagesText).ToList().GetRandomItem();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (message == null)
                        {
                            Dialog.ShowDialog(this, "Input Error",
                                "please input at least one message for Query [ " + item.Content.QueryValue + " ]");
                            return false;
                        }
                    }

            return true;
        }

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
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessageConfig.xaml
    /// </summary>
    public partial class AutoReplyToNewMessageConfig : AutoReplyToNewMessageConfigBase
    {
        public AutoReplyToNewMessageConfig()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AutoReplyToNewMessage,
                Enums.GdMainModule.InstaChat.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
        }

        private static AutoReplyToNewMessageConfig CurrentAutoReplyToNewMessageConfig { get; set; }

        public static AutoReplyToNewMessageConfig GetSingeltonObjectSendMessageToFollowerConfig()
        {
            return CurrentAutoReplyToNewMessageConfig ??
                   (CurrentAutoReplyToNewMessageConfig = new AutoReplyToNewMessageConfig());
        }

        private void ChkMessagesContainsSpecificWords_OnUnchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ObjViewModel.AutoReplyToNewMessageModel.SpecificWord = string.Empty;
                var count = ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Count;
                while (count > 1)
                {
                    var Content = ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries[count - 1]
                        .Content;
                    if (Content.QueryValue != "Default" &&
                        Content.QueryValue != FindResource("LangKeyReplyToAllMessages").ToString())
                        ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.RemoveAt(count - 1);

                    count--;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}