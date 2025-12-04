using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherMultiplePost.xaml
    /// </summary>
    public partial class PublisherMultiplePost : UserControl, INotifyPropertyChanged, IDisposable
    {
        private static PublisherMultiplePost _instance;

        private PublisherMultiplePostViewModel _publisherMultiplePostViewModel;

        public PublisherMultiplePost()
        {
            InitializeComponent();
            PublisherMultiplePostViewModel = new PublisherMultiplePostViewModel();
            MultiplePost.DataContext = PublisherMultiplePostViewModel;
            _instance = this;
        }

        public PublisherMultiplePost(ObservableCollection<PostDetailsModel> postDetails) : this()
        {
            PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.LstPostDetailsModels = postDetails;
        }

        public PublisherMultiplePostViewModel PublisherMultiplePostViewModel
        {
            get => _publisherMultiplePostViewModel;
            set
            {
                if (_publisherMultiplePostViewModel == value)
                    return;
                _publisherMultiplePostViewModel = value;
                OnPropertyChanged(nameof(PublisherMultiplePostViewModel));
            }
        }

        public void Dispose()
        {
            _instance = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherMultiplePost GetMultiplePost(ObservableCollection<PostDetailsModel> postDetails)
        {
            return _instance = _instance ?? (_instance = new PublisherMultiplePost(postDetails));
        }

        public ObservableCollection<PostDetailsModel> GetFinalPostDetails()
        {
            return PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.LstPostDetailsModels;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void PublisherMultiplePost_OnLoaded(object sender, RoutedEventArgs e)
        {
            //var lstPostDetails = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
            //        .PublisherCreateCampaignModel.LstPostDetailsModels;
            //PublisherMultiplePostViewModel.LstPostDetailsModel = lstPostDetails;

            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublisherMultiplePostViewModel.PostListsCollectionView =
                        CollectionViewSource.GetDefaultView(PublisherCreateCampaigns
                            .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                            .PublisherCreateCampaignModel.LstPostDetailsModels);
                });
            else
                PublisherMultiplePostViewModel.PostListsCollectionView =
                    CollectionViewSource.GetDefaultView(PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                        .PublisherCreateCampaignViewModel
                        .PublisherCreateCampaignModel.LstPostDetailsModels);
        }
    }
}