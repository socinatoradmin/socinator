using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    ///     Interaction logic for PublisherEditSourceUrl.xaml
    /// </summary>
    public partial class PublisherEditShareUrl : UserControl
    {
        private readonly PublisherPostlistModel currentPostData;
        private readonly ObservableCollection<PublisherPostlistModel> lstPublisherPostlist;

        public PublisherEditShareUrl()
        {
            InitializeComponent();
        }

        public PublisherEditShareUrl(PublisherPostlistModel currentPost,
            ObservableCollection<PublisherPostlistModel> lstPublisherPostlist) : this()
        {
            currentPostData = currentPost.DeepClone();
            this.lstPublisherPostlist = lstPublisherPostlist;
            DataContext = currentPostData;
        }

        private void PublisherEditSourceUrl_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaViewerControl.Initialize();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var indexToUpdate = lstPublisherPostlist.FindIndex(posts => posts.PostId == currentPostData.PostId);
                lstPublisherPostlist[indexToUpdate] = currentPostData;
                PostlistFileManager.UpdatePostlists(currentPostData.CampaignId, lstPublisherPostlist);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            Dialog.CloseDialog(sender);
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Dialog.CloseDialog(sender);
        }
    }
}