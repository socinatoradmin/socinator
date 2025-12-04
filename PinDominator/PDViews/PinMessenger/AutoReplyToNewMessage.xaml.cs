using System.Linq;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;
using PinDominatorCore.PDViewModel.PinMessenger;
using static PinDominatorCore.PDEnums.Enums;

namespace PinDominator.PDViews.PinMessenger
{
    public class
        AutoReplyToNewMessageBase : ModuleSettingsUserControl<AutoReplyToNewMessageViewModel, AutoReplyToNewMessageModel
        >
    {
        protected override bool ValidateCampaign()
        {
            if (ObjViewModel.AutoReplyToNewMessageModel.LstDisplayManageMessagesModel.Count == 0)
            {
                Dialog.ShowDialog(this, "Error", "Please add at least one Message.");
                return false;
            }

            return base.ValidateCampaign();
        }
    }

    /// <summary>
    ///     Interaction logic for AutoReplyToNewMessage.xaml
    /// </summary>
    public sealed partial class AutoReplyToNewMessage
    {
        public AutoReplyToNewMessage()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: AutoReplyToNewMessageHeader,
                footer: AutoReplyToNewMessageFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.AutoReplyToNewMessage,
                moduleName: PdMainModule.PinMessenger.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.AutoReplyToNewMessageVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.AutoReplyToNewMessageKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static AutoReplyToNewMessage CurrentAutoReplyToNewMessage { get; set; }

        public static AutoReplyToNewMessage GetSingletonObjectAutoReplyToNewMessage()
        {
            return CurrentAutoReplyToNewMessage ?? (CurrentAutoReplyToNewMessage = new AutoReplyToNewMessage());
        }

        private void ReplyToMessagesThatContainsSpecificWord_CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ObjViewModel.AutoReplyToNewMessageModel.IsCheckedReplyToMessagesThatContainsSpecificWord = true;

            var isExist = false;
            ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.ForEach(x =>
            {
                if (x.Content.QueryValue == Application.Current
                        .FindResource("LangKeyReplyToMessagesThatContainsSpecificWord")?.ToString())
                    isExist = true;
            });
            if (!isExist)
                ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Add(new QueryContent
                {
                    Content = new QueryInfo
                    {
                        QueryType = "Message Filter",
                        QueryValue = Application.Current.FindResource("LangKeyReplyToMessagesThatContainsSpecificWord")
                            ?.ToString()
                    }
                });
            if (ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.Any(x =>
                x.IsContentSelected == false))
                ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries
                    .FirstOrDefault(x => x.Content.QueryType.Equals("All")).IsContentSelected = false;
        }

        private void ReplyToMessagesThatContainsSpecificWord_CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ObjViewModel.AutoReplyToNewMessageModel.IsCheckedReplyToMessagesThatContainsSpecificWord = false;
            var delete = ObjViewModel.AutoReplyToNewMessageModel.ManageMessagesModel.LstQueries.FirstOrDefault(x =>
                x.Content.QueryValue == Application.Current
                    .FindResource("LangKeyReplyToMessagesThatContainsSpecificWord")?.ToString());

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