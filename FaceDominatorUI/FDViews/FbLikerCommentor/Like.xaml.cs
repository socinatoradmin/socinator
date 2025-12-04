using DominatorHouseCore.Annotations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDModel.CustomControlModel;
using FaceDominatorCore.FDViewModel.LikerCommentorViewModel;
using FaceDominatorUI.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using FaceDominatorCore.FDLibrary;

namespace FaceDominatorUI.FDViews.FbLikerCommentor
{
    /// <summary>
    /// Interaction logic for Like.xaml
    /// </summary>
    public partial class Like : UserControl, INotifyPropertyChanged, IHeaderControl, IHelpControl, IFooterControl
    {
        public Like()
        {
            InitializeComponent();
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }


        private static Like CurrentLike { get; set; } = null;

        public static Like GetSingeltonObjectLike()
        {
            return CurrentLike ?? (CurrentLike = new Like());
        }


        public event PropertyChangedEventHandler PropertyChanged;


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        #region IHeaderControl

        private string _campaignName;

        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                _campaignName = value;
                OnPropertyChanged(nameof(CampaignName));
            }
        }

        private bool _isEditCampaignName = true;

        public bool IsEditCampaignName
        {
            get
            {
                return _isEditCampaignName;
            }
            set
            {
                _isEditCampaignName = value;
                OnPropertyChanged(nameof(IsEditCampaignName));
            }
        }


        private Visibility _cancelEditVisibility = Visibility.Collapsed;

        public Visibility CancelEditVisibility
        {
            get
            {
                return _cancelEditVisibility;
            }
            set
            {
                _cancelEditVisibility = value;
                OnPropertyChanged(nameof(CancelEditVisibility));
            }
        }
        private string _templateId;
        public string TemplateId
        {
            get
            {
                return _templateId;
            }

            set
            {
                _templateId = value;
                OnPropertyChanged(nameof(_templateId));
            }
        }

        #endregion

        #region IHelpControl

        public string VideoTutorialLink { get; set; } = FdConstants.ManageFriendsVideoTutorialsLink;

        public string KnowledgeBaseLink { get; set; } = FdConstants.ManageFriendsKnowledgeBaseLink;

        public string ContactSupportLink { get; set; } = FdConstants.ManageFriendsContactLink;

        #endregion

        #region IFooterControl

        private string _selectedAccountCount = ConstantVariable.NoAccountSelected;

        public string SelectedAccountCount
        {
            get
            {
                return _selectedAccountCount;
            }
            set
            {
                _selectedAccountCount = value;
                OnPropertyChanged(nameof(SelectedAccountCount));
            }
        }
        private string _campaignButtonContent = ConstantVariable.CreateCampaign;
        public string CampaignButtonContent
        {
            get
            {
                return _campaignButtonContent;
            }
            set
            {
                _campaignButtonContent = value;
                OnPropertyChanged(nameof(CampaignButtonContent));
            }
        }

        #endregion


        private void HeaderControl_OnInfoChanged(object sender, RoutedEventArgs e)
        {
            HelpFlyout.IsOpen = true;
        }

        

        private void LikeFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        {

        }

        private void LikeFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        {

        }

        private void LikerSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            UpdateViewModelForSearchQueryChanges(ObjLikeViewModel, LikerSearchControl);
            LikerSearchControl.CurrentQuery = new CustomQueryInfo();
        }

       

        private void LikerSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        {

        }

        private LikeViewModel _objLikeViewModel;
        public LikeViewModel ObjLikeViewModel
        {
            get
            {
                return _objLikeViewModel;
            }
            set
            {
                _objLikeViewModel = value;
                OnPropertyChanged(nameof(ObjLikeViewModel));
            }
        }


        private void SetDataContext()
        {
            this.SelectedAccountCount = ConstantVariable.NoAccountSelected;
            ObjLikeViewModel = new LikeViewModel();
            LikeFooter.list_SelectedAccounts = new List<string>();
            MainGrid.DataContext = ObjLikeViewModel.LikeModel;
            HeaderGrid.DataContext = FooterGrid.DataContext = this;
            CampaignName = $"FD Manage Friend Request [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
        }

        public static void UpdateViewModelForSearchQueryChanges(LikeViewModel viewModel, CustomSearchQueryControl queryControl)
        {
            if (string.IsNullOrEmpty(queryControl.CurrentQuery.QueryValue))
            {
                var currentQuery = queryControl.CurrentQuery.Clone() as CustomQueryInfo;

                queryControl.QueryCollection.ForEach(query =>
                {
                    currentQuery = queryControl.CurrentQuery.Clone() as CustomQueryInfo;
                    if (currentQuery == null)
                        return;

                    currentQuery.QueryValue = query;
                    currentQuery.QueryTypeDisplayName
                        = currentQuery.QueryType;
                    currentQuery.QueryPriority
                        = viewModel.LikeModel.SavedQueries.Count + 1;
                    viewModel.LikeModel.SavedQueries.Add(currentQuery);
                });
            }
            else
            {
                queryControl.CurrentQuery.QueryTypeDisplayName
                = queryControl.CurrentQuery.QueryType.ToString();
                var currentQuery = queryControl.CurrentQuery.Clone() as CustomQueryInfo;
                if (currentQuery == null)
                    return;
                currentQuery.QueryPriority = viewModel.LikeModel.SavedQueries.Count + 1;
                viewModel.LikeModel.SavedQueries.Add(currentQuery);
                queryControl.CurrentQuery = new CustomQueryInfo();
            }

        }

        // To switch the filters for fanpage liker and Post Liker
        private void LikerSearchControl_OnEngagementChanged(object sender, RoutedEventArgs e)
        {
            var currentQuery = LikerSearchControl.CurrentQuery.Clone() as CustomQueryInfo;

            if(currentQuery.EngagementType== "Post")
            {
                ObjLikeViewModel.LikeModel.IsFanpageFilterVisible = false;
                ObjLikeViewModel.LikeModel.IsPostFilterVisible = true;
            }
            else if(currentQuery.EngagementType == "Fanpage")
            {
                ObjLikeViewModel.LikeModel.IsFanpageFilterVisible = true;
                ObjLikeViewModel.LikeModel.IsPostFilterVisible = false;
            }
            else
            {
                ObjLikeViewModel.LikeModel.IsFanpageFilterVisible = false;
                ObjLikeViewModel.LikeModel.IsPostFilterVisible = false;
            }
        }
    }
}
