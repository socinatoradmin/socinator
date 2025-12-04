using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDEnums;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinMessenger;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PinDominator.PDViews.Tools.AutoReplyToNewMessage
{
    public class AutoReplyToNewMessageConfigurationBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel,
        AutoReplyToNewMessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (ObjViewModel.AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one message.");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.AutoReplyToNewMessageModel =
                        templateModel.ActivitySettings.GetActivityModel<AutoReplyToNewMessageModel>(ObjViewModel.Model,
                            true);
                else if (ObjViewModel == null)
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
    ///     Interaction logic for AutoReplyToNewMessageConfiguration.xaml
    /// </summary>
    public partial class AutoReplyToNewMessageConfiguration
    {
        private readonly QueryContent _queryContent = new QueryContent {Content = new QueryInfo {QueryValue = "All"}};

        public AutoReplyToNewMessageConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.AutoReplyToNewMessage,
                Enums.PdMainModule.PinMessenger.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AutoReplyToNewMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            MainGrid.DataContext = ObjViewModel.Model;

            ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(_queryContent);
            ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "Message Filter",
                    QueryValue = Application.Current
                        .FindResource(
                            "LangKeyReplyToNewPendingMessagesReplyOnlyMessageSentByUsersThatDontFollowYourAccount")
                        ?.ToString()
                }
            });
        }

        private static AutoReplyToNewMessageConfiguration CurrentAutoReplyToNewMessageConfiguration { get; set; }

        /// <summary>
        ///     GetSingletonObjectAutoReplyToNewMessageConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static AutoReplyToNewMessageConfiguration GetSingletonObjectAutoReplyToNewMessageConfiguration()
        {
            return CurrentAutoReplyToNewMessageConfiguration ??
                   (CurrentAutoReplyToNewMessageConfiguration = new AutoReplyToNewMessageConfiguration());
        }

        private void ReplyToMessagesThatContainsSpecificWordText_GetInputClick(object sender, RoutedEventArgs e)
        {
            var specificWords = sender as InputBoxControl;
            if (specificWords != null)
                ObjViewModel.AutoReplyToNewMessageModel.LstMessagesContainsSpecificWords =
                    Regex.Split(specificWords.InputText, "\r\n").ToList();
            GlobusLogHelper.log.Info("Messages that contains specific word details stored successfully");
        }

        private void ReplyToMessagesThatContainsSpecificWord_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
            {
                Content = new QueryInfo
                {
                    QueryType = "Message Filter",
                    QueryValue = Application.Current.FindResource("LangKeyReplyToMessagesThatContainsSpecificWord")
                        .ToString()
                }
            });

            if (ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Any(x =>
                x.IsContentSelected == false))
                ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries
                    .FirstOrDefault(x => x.Content.QueryType.Equals("All")).IsContentSelected = false;
        }

        private void ReplyToMessagesThatContainsSpecificWord_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var delete = ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                x.Content.QueryValue == Application.Current
                    .FindResource("LangKeyReplyToMessagesThatContainsSpecificWord").ToString());

            ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Remove(delete);
            ObjViewModel.AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.ForEach(item =>
                item.SelectedQuery.Remove(delete));

            if (ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Any(x =>
                x.IsContentSelected == false))
                ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries
                    .FirstOrDefault(x => x.Content.QueryType.Equals("All")).IsContentSelected = false;
        }
    }
}