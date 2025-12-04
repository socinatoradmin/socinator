using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorUIUtility.ViewModel.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublisherDirectPosts.xaml
    /// </summary>
    public partial class PublisherDirectPosts : INotifyPropertyChanged
    {
        private static PublisherDirectPosts instance;

        private PublisherDirectPostsViewModel _publisherDirectPostsViewModel;

        public PublisherDirectPosts()
        {
            InitializeComponent();
            instance = this;
        }

        public PublisherDirectPosts(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            PublisherDirectPostsViewModel = new PublisherDirectPostsViewModel(tabItemsControl);
            tabItemsControl.PublisherDirectPostsViewModel = PublisherDirectPostsViewModel;
            DirectPost.DataContext = PublisherDirectPostsViewModel;
        }

        public PublisherDirectPostsViewModel PublisherDirectPostsViewModel
        {
            get => _publisherDirectPostsViewModel;
            set
            {
                _publisherDirectPostsViewModel = value;
                OnPropertyChanged(nameof(PublisherDirectPostsViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public static PublisherDirectPosts GetPublisherDirectPosts(
            PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            return instance ?? (instance = new PublisherDirectPosts(tabItemsControl));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ImageMediaViewer_OnDeleteImage(object sender, RoutedEventArgs e)
        {
            try
            {
                var mediaViewer = (MediaViewer) sender;

                var postDetails = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                    .PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels;

                var notAvailableMedias = PublisherDirectPostsViewModel.MediaList.Except(mediaViewer.MediaList).ToList();

                notAvailableMedias.ForEach(y =>
                {
                    postDetails.Remove(postDetails.FirstOrDefault(x => x.MediaList.Contains(y)));
                });

                PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels = postDetails;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}