using DominatorHouseCore.Annotations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using DominatorHouseCore.Enums;
using YoutubeDominatorUI.Utility;
using static YoutubeDominatorCore.YDEnums.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore;

namespace YoutubeDominatorUI.YDViews.Engage
{
    public class ReplyToComment_Base : ModuleSettingsUserControl<ReplyToCommentViewModel, ReplyToCommentModel>
    {
        public override void AddNewCampaign(List<string> lstSelectedAccounts, ActivityType moduleType) { }// => GlobalMethods.AddNewCampaign(lstSelectedAccounts, moduleType);

        public override void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType) { }// => GlobalMethods.SaveDetails(lstSelectedAccounts, moduleType);
    }

    public partial class ReplyToComment : ReplyToComment_Base
    {

        private static ReplyToComment Current_ReplyToComment { get; set; } = null;
        public static ReplyToComment GetSingeltonObjectReplyToComment()
        {
            return Current_ReplyToComment ?? (Current_ReplyToComment = new ReplyToComment());
        }
        public ReplyToComment()
        {

            InitializeComponent();

            base.InitializeBaseClass
            (
                header: HeaderGrid,
                footer: ReplyToCommentFooter,
                queryControl: ReplyToCommentSearchControl,
                MainGrid: MainGrid,
                //activityType: ActivityType.ReplyToComment,
                activityType: ActivityType.ReplyToComment,
                moduleName: YdMainModule.ReplyToComment.ToString()
            );

            Current_ReplyToComment = this;
            try
            {

                base.SetDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        private ReplyToCommentViewModel _objReplyToCommentViewModel;

        public ReplyToCommentViewModel ObjReplyToCommentViewModel
        {
            get
            {
                return _objReplyToCommentViewModel;
            }
            set
            {
                _objReplyToCommentViewModel = value;
                OnPropertyChanged(nameof(ObjReplyToCommentViewModel));
            }
        }





        private void HeaderGrid_OnCancelEditClick(object sender, RoutedEventArgs e)
        {

        }

        private void ReplyToCommentSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnCustomFilterChanged(sender, e);


        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        private void ReplyToCommentFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
            => base.FooterControl_OnSelectAccountChanged(sender, e);


        private void ReplyToCommentFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
            => base.CreateCampaign();
            //=> base.FooterControl_OnCreateCampaignChanged(sender, e, Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithReplytocommentConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString(), ObjViewModel.Model.JobConfiguration.RunningTime);
        //=> base.FooterControl_OnCreateCampaignChanged(sender, e);

        private void ReplyToCommentFooter_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
            => UpdateCampaign();
            //=> base.FooterControl_OnUpdateCampaignChanged(sender, e);

        private void ReplyToCommentSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnAddQuery(sender, e, typeof(YdScraperParameters));

    }
}
