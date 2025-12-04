using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaPoster;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.InstaPoster
{
    public class PosterBase : ModuleSettingsUserControl<PosterViewModel, PosterModel>
    {
    }

    /// <summary>
    ///     Interaction logic for Poster.xaml
    /// </summary>
    public partial class Poster : PosterBase
    {
        private Poster()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: PosterHeader,
                footer: PosterFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.Post,
                moduleName: Enums.GdMainModule.Poster.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.PosterVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.PosterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Poster CurrentPoster { get; set; }

        /// <summary>
        ///     GetSingeltonObjectPoster is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Poster GetSingeltonObjectPoster()
        {
            return CurrentPoster ?? (CurrentPoster = new Poster());
        }

        private void PosterHeader_OnCancelEditClick(object sender, RoutedEventArgs e)
        {
            HeaderControl_OnCancelEditClick(sender, e);
        }

        private void PosterHeader_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        private void PosterFooter_OnSelectAccountChanged(object sender, RoutedEventArgs e)
        {
            FooterControl_OnSelectAccountChanged(sender, e);
        }

        // TODO : This module no longer needed. Delete this class when Publisher module will get completed
        private void PosterFooter_OnCreateCampaignChanged(object sender, RoutedEventArgs e)
        {
            CreateCampaign();
        }

        private void PosterFooter_OnUpdateCampaignChanged(object sender, RoutedEventArgs e)
        {
            UpdateCampaign();
        }

        private void BtnLoadFromCSV_OnClick(object sender, RoutedEventArgs e)
        {
            var v = FileUtilities.FileBrowseAndReader();
        }
    }
}