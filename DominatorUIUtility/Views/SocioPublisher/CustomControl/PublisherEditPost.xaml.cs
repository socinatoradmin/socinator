using System;
using System.Collections.ObjectModel;
using System.Threading;
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
    ///     Interaction logic for PublisherEditPost.xaml
    /// </summary>
    public partial class PublisherEditPost : UserControl
    {
        public PublisherEditPost()
        {
            InitializeComponent();
        }

        public PublisherEditPost(PublisherPostlistModel publisherPostlistModel,
            ObservableCollection<PublisherPostlistModel> LstPostListModel) : this()
        {
            this.LstPostListModel = LstPostListModel;
            PostlistModel = publisherPostlistModel.DeepClone();
            DataContext = PostlistModel;
        }

        private PublisherPostlistModel PostlistModel { get; }
        private ObservableCollection<PublisherPostlistModel> LstPostListModel { get; }

        private void PublisherEditPost_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PostContentControl.SetMedia();
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
                var indexToUpdate = LstPostListModel.FindIndex(posts => posts.PostId == PostlistModel.PostId);


                var postDetails = LstPostListModel[indexToUpdate];

                var readdCount = PostlistModel.PublisherPostSettings.GeneralPostSettings.ReaddCount -
                                 postDetails.PublisherPostSettings.GeneralPostSettings.ReaddCount;
                Thread.Sleep(10);
                PostlistModel.InitializePostData();

                if (PostlistModel.PublisherPostSettings.GeneralPostSettings.IsReaddCount && readdCount > 0)
                    for (var readdInitial = 0; readdInitial < readdCount; readdInitial++)
                    {
                        var deepClonePost = PostlistModel.DeepClone();
                        deepClonePost.GenerateClonePostId();
                        deepClonePost.PublisherPostSettings.GeneralPostSettings.ReaddCount = 1;
                        deepClonePost.PublisherPostSettings.GeneralPostSettings.IsReaddCount = false;
                        LstPostListModel.Add(deepClonePost);
                    }

                PostlistModel.PublisherPostSettings.GeneralPostSettings.ReaddCount = 1;
                PostlistModel.PublisherPostSettings.GeneralPostSettings.IsReaddCount = false;

                LstPostListModel[indexToUpdate] = PostlistModel;


                PostlistFileManager.UpdatePostlists(PostlistModel.CampaignId, LstPostListModel);
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