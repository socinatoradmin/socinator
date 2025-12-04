using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaScraper;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.InstaScrape
{
    public class DownloadPhotosBase : ModuleSettingsUserControl<DownloadPhotosViewModel, DownloadPhotosModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.PostFilterModel.PostCategory.FilterPostCategory)
                if (Model.PostFilterModel.PostCategory.IgnorePostImages &&
                    Model.PostFilterModel.PostCategory.IgnorePostVideos &&
                    Model.PostFilterModel.PostCategory.IgnorePostAlbums)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please check maximum two options for Post Type filteration inside Post Filter category.");
                    return false;
                }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for DownloadPhotos.xaml
    /// </summary>
    public partial class DownloadPhotos : DownloadPhotosBase
    {
        private DownloadPhotos()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: DownloadPhotosHeader,
                footer: DownloadPhotosFooterControl,
                queryControl: DownloadPhotosSearchQueryControl,
                MainGrid: MainGrid,
                activityType: ActivityType.PostScraper,
                moduleName: Enums.GdMainModule.Scraper.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.DownloadPhotosVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.DownloadPhotosKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static DownloadPhotos CurrentDownloadPhotos { get; set; }

        /// <summary>
        ///     GetSingeltonObjectDownloadPhotos is used to get the object of the current user control,
        ///     if object is already created then it won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static DownloadPhotos GetSingeltonObjectDownloadPhotos()
        {
            return CurrentDownloadPhotos ?? (CurrentDownloadPhotos = new DownloadPhotos());
        }

        private void Chk_RequiredData(object sender, RoutedEventArgs e)
        {
            ObjViewModel.CheckAllReqData(sender);
        }
    }
}