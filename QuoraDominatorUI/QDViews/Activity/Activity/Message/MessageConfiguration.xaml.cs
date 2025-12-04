using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorUIUtility.CustomControl;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.ViewModel.Messages;
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

namespace QuoraDominatorUI.QDViews.Activity.Message
{
    /// <summary>
    /// Interaction logic for MessageConfiguration.xaml
    /// </summary>
    /// 

    public class MessageConfigurationBase : ModuleSettingsUserControl<BroadcastMessagesViewModel, BroadcastMessagesModel>
    {
        public void AccountGrowthHeader_SaveClick(object sender, RoutedEventArgs e) => ObjViewModel.BroadcastMessagesModel.IsAccountGrowthActive = base.SaveAccountGrowthSettings();

    }
    public partial class MessageConfiguration : MessageConfigurationBase
    {
        public MessageConfiguration()
        {
            InitializeComponent();
        }
        private void TglStatus_OnIsCheckedChanged(object sender, EventArgs e) => base.ScheduleJobFromGrowthMode(ObjViewModel.Model.IsAccountGrowthActive, AccountGrowthHeader.SelectedItem, SocialNetworks.Quora);
        private void FollowConfigurationSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
          => base.SearchQueryControl_OnAddQuery(sender, e);

        private void AccountGrowthHeader_OnSaveClick(object sender, RoutedEventArgs e) => ObjViewModel.BroadcastMessagesModel.IsAccountGrowthActive = base.SaveAccountGrowthSettings();
        private void MessagesControl_OnAddMessagesToListChanged(object sender, RoutedEventArgs e)
        {
            var messageData = sender as MessagesControl;

            if (messageData == null) return;
            messageData.Messages.SerialNo = ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessageModel.Count + 1;
            messageData.Messages.SelectedQuery.Remove(messageData.Messages.SelectedQuery.FirstOrDefault(x => x.Content.QueryValue == "All"));

            ObjViewModel.BroadcastMessagesModel.LstDisplayManageMessageModel.Add(messageData.Messages);

            messageData.Messages = new ManageMessagesModel();

            messageData.Messages.LstQueries = ObjViewModel.BroadcastMessagesModel.ManageMessagesModel.LstQueries;
            messageData.Messages.LstQueries.Select(query => { query.IsContentSelected = false; return query; }).ToList();

            ObjViewModel.BroadcastMessagesModel.ManageMessagesModel = messageData.Messages;
            messageData.ComboBoxQueries.ItemsSource = ObjViewModel.BroadcastMessagesModel.ManageMessagesModel.LstQueries;
        }
    }
}
