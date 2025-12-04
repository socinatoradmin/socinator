using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.GrowFollower;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.GrowFollowers
{
    public class FollowerBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please add at least one query.");
                return false;
            }

            // Check AutoFollow.Unfollow
            if (Model.IsChkEnableAutoFollowUnfollowChecked)
                if (!Model.IsChkStopFollowToolWhenReachedSpecifiedFollowings &&
                    !Model.IsChkWhenFollowerFollowingsIsSmallerThanChecked)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please select atleast one checkbox option inside AutoFollow/Unfollow feature to  Stat/Stop Unfollow/Follow process.");
                    return false;
                }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for Follower.xaml
    /// </summary>
    public partial class Follower : FollowerBase
    {
        /// Constructor
        private Follower()
        {
            InitializeComponent();


            InitializeBaseClass
            (
                header: FollowHeader,
                footer: FollowFooter,
                queryControl: FollowerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Follow,
                moduleName: GdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Follower CurrentFollower { get; set; }

        /// <summary>
        ///     GetSingeltonObjectFollower is used to get the object of the current user control,
        ///     if object is already created then it won't create a new object, simply it returns already created object,
        ///     otherwise will return a new created object.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Follower GetSingeltonObjectFollower()
        {
            return CurrentFollower ?? (CurrentFollower = new Follower());
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value) ObjViewModel.Model.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Model.MediaPath = "";
        }
    }
}