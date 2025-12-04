using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.Instachat;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Instachats
{
    public class
        AutoReplyToNewMessageBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel, AutoReplyToNewMessageModel
        >
    {
        protected override bool ValidateCampaign()
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
                foreach (var queryContent in Model.ManageMessagesModel.LstQueries)
                    if (queryContent.Content.QueryValue != "All")
                    {
                        string message = null;
                        try
                        {
                            message = ObjViewModel.AutoReplyToNewMessageModel.LstDisplayManageMessageModel
                                .Where(x => x.SelectedQuery.Any(y =>
                                    y.Content.QueryValue.ToString() == queryContent.Content.QueryValue))
                                .Select(x => x.MessagesText).ToList().GetRandomItem();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (message == null)
                        {
                            Dialog.ShowDialog(this, "Input Error",
                                $"please input at least one message for Query [ {queryContent.Content.QueryValue} ]");
                            return false;
                        }
                    }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessage.xaml
    /// </summary>
    public partial class AutoReplyToNewMessage : AutoReplyToNewMessageBase
    {
        public AutoReplyToNewMessage()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: HeaderControl,
                footer: AutoReplyToNewMessageFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoReplyToNewMessage,
                moduleName: Enums.GdMainModule.InstaChat.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AutoReplyToNewMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static AutoReplyToNewMessage CurrentAutoReplyToNewMessage { get; set; }

        /// <summary>
        ///     GetSingeltonAutoReplyToNewMessage is used to get the object of the current user control,
        ///     if object is already created then it won't create a new object, simply it returns already created object,
        ///     otherwise will return a new created object.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static AutoReplyToNewMessage GetSingeltonAutoReplyToNewMessage()
        {
            return CurrentAutoReplyToNewMessage ?? (CurrentAutoReplyToNewMessage = new AutoReplyToNewMessage());
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