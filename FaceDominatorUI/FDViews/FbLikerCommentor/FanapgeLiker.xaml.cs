using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDViewModel.LikerCommentorViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbLikerCommentor
{
    public class FanapgeLikerBase : ModuleSettingsUserControl<FanpageLikerViewModel, FanpageLikerModel>
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

            if (!Model.IsActionasOwnAccountChecked && !Model.IsActionasPageChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectReactByOwnAccountOrPage".FromResourceDictionary());
                return false;
            }
            if (Model.IsActionasPageChecked && Model.ListOwnPageUrl.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                   "LangKeySelectAtleastOneOwnPage".FromResourceDictionary());
                return false;
            }
            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for FanapgeLiker.xaml
    /// </summary>
    public partial class FanapgeLiker
    {
        public FanapgeLiker()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: LikeFooter,
                queryControl: LikerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.FanpageLiker,
                moduleName: FdMainModule.LikerCommentor.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.FanpageLikerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.FanpageLikerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static FanapgeLiker CurrentFanapgeLiker { get; set; }

        public static FanapgeLiker GetSingeltonObjectFanapgeLiker()
        {
            return CurrentFanapgeLiker ?? (CurrentFanapgeLiker = new FanapgeLiker());
        }

        #region OldEvents

        //private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        //{
        //    HelpFlyout.IsOpen = true;
        //}


        //private void LikerSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        //{
        //    FanpageFilterControl objUserFiltersControl = new FanpageFilterControl();

        //    objUserFiltersControl.IsSaveCloseButtonVisisble = true;

        //    if (!string.IsNullOrEmpty(LikerSearchControl.CurrentQuery.CustomFilters))
        //    {
        //        try
        //        {
        //            objUserFiltersControl.FanpageFilter = JsonConvert.DeserializeObject<FdFanpageFilterModel>(LikerSearchControl.CurrentQuery.CustomFilters);

        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    else
        //    {

        //        objUserFiltersControl.FanpageFilter = new FdFanpageFilterModel();
        //    }


        //    Dialog objDialog = new Dialog();

        //    var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

        //    objUserFiltersControl.SaveButton.Click += (senders, Events) =>
        //    {

        //        var UserFilter = objUserFiltersControl.FanpageFilter;
        //        var SerializeCustomFilter = JsonConvert.SerializeObject(UserFilter);
        //        LikerSearchControl.CurrentQuery.CustomFilters = SerializeCustomFilter;

        //        FilterWindow.Close();
        //    };

        //    FilterWindow.ShowDialog();
        //}

        //private void HeaderOnCancelEdit_Click(object sender, RoutedEventArgs e)
        //{
        //    base.HeaderControl_OnCancelEditClick(sender, e);
        //    TabSwitcher.GoToCampaign();
        //} 

        #endregion
    }
}