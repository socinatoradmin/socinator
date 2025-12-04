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
    /// Interaction logic for Comment.xaml
    /// </summary>
    public partial class Comment : UserControl, INotifyPropertyChanged, IHeaderControl, IHelpControl, IFooterControl
    {
        public Comment()
        {
            InitializeComponent();
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Comment CurrentComment { get; set; } = null;

        public static Comment GetSingeltonObjectComment()
        {
            return CurrentComment ?? (CurrentComment = new Comment());
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

      

        private void CommentFooter_SelectAccountChanged(object sender, RoutedEventArgs e)
        {

        }

        private void CommentFooter_CreateCampaignChanged(object sender, RoutedEventArgs e)
        {

        }

        private void CommentSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            UpdateViewModelForSearchQueryChanges(ObjCommentViewModel, CommentorSearchControl);
            CommentorSearchControl.CurrentQuery = new CustomQueryInfo();
        }

      
        private void CommentSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        {

        }

        private CommentViewModel _objCommentViewModel;
        public CommentViewModel ObjCommentViewModel
        {
            get
            {
                return _objCommentViewModel;
            }
            set
            {
                _objCommentViewModel = value;
                OnPropertyChanged(nameof(ObjCommentViewModel));
            }
        }


        private void SetDataContext()
        {
            this.SelectedAccountCount = ConstantVariable.NoAccountSelected;
            ObjCommentViewModel = new CommentViewModel();
            CommentFooter.list_SelectedAccounts = new List<string>();
            MainGrid.DataContext = ObjCommentViewModel.CommentModel;
            HeaderGrid.DataContext = FooterGrid.DataContext = this;
            CampaignName = $"FD Manage Friend Request [{DateTime.Now.ToString(CultureInfo.InvariantCulture)}]";
        }

        public static void UpdateViewModelForSearchQueryChanges(CommentViewModel viewModel, CustomSearchQueryControl queryControl)
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
                        = viewModel.CommentModel.SavedQueries.Count + 1;
                    viewModel.CommentModel.SavedQueries.Add(currentQuery);
                });
            }
            else
            {

                queryControl.CurrentQuery.QueryTypeDisplayName
                = queryControl.CurrentQuery.QueryType.ToString();
                var currentQuery = queryControl.CurrentQuery.Clone() as CustomQueryInfo;
                if (currentQuery == null)
                    return;
                currentQuery.QueryPriority = viewModel.CommentModel.SavedQueries.Count + 1;
                viewModel.CommentModel.SavedQueries.Add(currentQuery);
                queryControl.CurrentQuery = new CustomQueryInfo();
            }

        }

       
    }
}
