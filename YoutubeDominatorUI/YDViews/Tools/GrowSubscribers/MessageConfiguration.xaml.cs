using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModel;
using YoutubeDominatorCore.YoutubeViewModel;
using YoutubeDominatorUI.CustomControl;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.Tools.GrowMessagers
{
    public class MessageConfigurationBase : ModuleSettingsUserControl<MessageViewModel, MessageModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", "Please add at least one query.",
                    MessageDialogStyle.Affirmative);
                return false;
            }
            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.messageModel =
                        JsonConvert.DeserializeObject<MessageModel>(templateModel.ActivitySettings);

                ObjViewModel.messageModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
    /// <summary>
    /// Interaction logic for EngageConfiguration.xaml
    /// </summary>
    public partial class MessageConfiguration : MessageConfigurationBase
    {
        public MessageConfiguration()
        {
            InitializeComponent();
            
            DialogParticipation.SetRegister(this, this);

            base.InitializeBaseClass
            (
                MainGrid: MainGrid,
                activityType: ActivityType.BroadcastMessages,
                moduleName: YdMainModule.Message.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: MessageSearchControl
            );

            // Help control links. 
            base.VideoTutorialLink = ConstantHelpDetails.MessageVideoTutorialsLink;
            base.KnowledgeBaseLink = ConstantHelpDetails.MessageKnowledgeBaseLink;
            base.ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            AccountGrowthHeader.AccountItemSource = new DominatorHouseCore.Utility.ObservableCollectionBase<string>(AccountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.messageModel;
        }

        private static MessageConfiguration CurrentMessageConfiguration = null;

        public static MessageConfiguration GetSingeltonObjectMessageConfiguration()
        {
            return CurrentMessageConfiguration ?? (CurrentMessageConfiguration = new MessageConfiguration());
        }

        private void AccountgrowthHeader_OnSaveClick(object sender, RoutedEventArgs e)
             => SaveConfigurations();
        //{
        //     base.AccountGrowthHeader_OnSaveClick(sender, e);
        //}

        private void accountgrowthHeader_SelectionChanged(object sender, RoutedEventArgs e)
        {
            base.SetAccountModeDataContext(SocialNetworks.Youtube);
        }

        private void MessageSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            base.SearchQueryControl_OnAddQuery(sender, e, typeof(DominatorHouseCore.Enums.YdQuery.YdScraperParameters));
        }

        private void MessageSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)

        {
            UserFiltersControl objUserFiltersControl = new UserFiltersControl();
            Dialog objDialog = new Dialog();

            var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

            objUserFiltersControl.SaveButton.Click += (senders, Events) =>
            {

                var UserFilter = objUserFiltersControl.UserFilter;
                var SerializeCustomFilter = JsonConvert.SerializeObject(UserFilter);
                MessageSearchControl.CurrentQuery.CustomFilters = SerializeCustomFilter;

                FilterWindow.Close();
            };

            FilterWindow.ShowDialog();
        }

        private void Message_AddMessageToListChanged(object sender, RoutedEventArgs e)
        {
 
        }

        private void MessageConfigurationBase_Loaded(object sender, RoutedEventArgs e)
            => base.SetSelectedAccounts(SocialNetworks.Youtube);
            //=> base.SetSelectedAccounts(SocialNetworks.Youtube, SelectedDominatorAccounts.YdAccounts);

        private void TglStatus_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ChangeAccountsModuleStatus(ObjViewModel.messageModel.IsAccountGrowthActive, AccountGrowthHeader.SelectedItem, SocialNetworks.Youtube))
            {
                ObjViewModel.messageModel.IsAccountGrowthActive = false;
            }
        }
        //=> ChangeAccountsModuleStatus(
        //ObjViewModel.MessageModel.IsAccountGrowthActive, accountGrowthHeader.SelectedItem,
        //SocialNetworks.Youtube);
    }
}
