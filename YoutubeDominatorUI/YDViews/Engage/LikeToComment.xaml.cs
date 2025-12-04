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
    public class LikeToComment_Base : ModuleSettingsUserControl<LikeToCommentViewModel, LikeToCommentModel>
    {
        public override void AddNewCampaign(List<string> lstSelectedAccounts, ActivityType moduleType) { }// => GlobalMethods.AddNewCampaign(lstSelectedAccounts, moduleType);

        public override void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType) { }// => GlobalMethods.SaveDetails(lstSelectedAccounts, moduleType);
    }

    public partial class LikeToComment : LikeToComment_Base
    {

        private static LikeToComment Current_LikeToComment { get; set; } = null;
        public static LikeToComment GetSingeltonObjectLikeToComment()
        {
            return Current_LikeToComment ?? (Current_LikeToComment = new LikeToComment());
        }
        public LikeToComment()
        {

            InitializeComponent();

            base.InitializeBaseClass
            (
                header: HeaderGrid,
                footer: LikeToCommentFooter,
                queryControl: LikeToCommentSearchControl,
                MainGrid: MainGrid,
                //activityType: ActivityType.LikeToComment,
                activityType: ActivityType.Like,
                moduleName: YdMainModule.LikeToComment.ToString()
            );

            Current_LikeToComment = this;
            try
            {

                base.SetDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        private LikeToCommentViewModel _objLikeToCommentViewModel;

        public LikeToCommentViewModel ObjLikeToCommentViewModel
        {
            get
            {
                return _objLikeToCommentViewModel;
            }
            set
            {
                _objLikeToCommentViewModel = value;
                OnPropertyChanged(nameof(ObjLikeToCommentViewModel));
            }
        }





        private void HeaderGrid_OnCancelEditClick(object sender, RoutedEventArgs e)
        {

        }

        private void LikeToCommentSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnCustomFilterChanged(sender, e);


        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        private void LikeToCommentFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
            => base.FooterControl_OnSelectAccountChanged(sender, e);


        private void LikeToCommentFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
            => base.CreateCampaign();
        //=> base.FooterControl_OnCreateCampaignChanged(sender, e, Application.Current.FindResource("LangKeyTheseAccountsAreAlreadyRunningWithLiketocommentConfigurationFromAnotherCampaignSavingThisSettingsWillOverridePreviousSettingsAndRemoveThisAccountFromTheCampaign").ToString(), ObjViewModel.Model.JobConfiguration.RunningTime);
        //=> base.FooterControl_OnCreateCampaignChanged(sender, e);

        private void LikeToCommentFooter_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
            => UpdateCampaign();
        //=> base.FooterControl_OnUpdateCampaignChanged(sender, e);

        private void LikeToCommentSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
            => base.SearchQueryControl_OnAddQuery(sender, e, typeof(YdScraperParameters));

    }
}
