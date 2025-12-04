using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherMultiplePostViewModel : BindableBase
    {
        public PublisherMultiplePostViewModel()
        {
            //LstPostDetailsModel = new ObservableCollection<PostDetailsModel>();
            CreateNewPost = new BaseCommand<object>(CanExecuteCreateNewPost, ExecuteCreateNewPost);
            ImportFromCsvCommand = new BaseCommand<object>(ImportFromCsvCanExecute, ImportFromCsvExecute);
            DeletePostCommand = new BaseCommand<object>(DeletePostCanExecute, DeletePostExecute);
            StopLoadingPostCommand = new BaseCommand<object>(sender => true, StopLoadingPost);

            //BindingOperations.EnableCollectionSynchronization(LstPostDetailsModel, lockObject);
        }

        private void StopLoadingPost(object sender)
        {
            IsProgressVisibile = Visibility.Collapsed;
            IsProgressActive = false;
            IsStopLoadingPost = true;
        }

        #region Command

        public ICommand CreateNewPost { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }
        public ICommand DeletePostCommand { get; set; }
        public ICommand StopLoadingPostCommand { get; set; }

        #endregion

        #region Properties

        private static object lockObject = new object();
        private bool _isProgressActive;

        public bool IsProgressActive
        {
            get => _isProgressActive;
            set => SetProperty(ref _isProgressActive, value);
        }

        private bool _isStopLoadingPost;

        public bool IsStopLoadingPost
        {
            get => _isStopLoadingPost;
            set => SetProperty(ref _isStopLoadingPost, value);
        }

        private Visibility _isProgressVisibile = Visibility.Collapsed;

        public Visibility IsProgressVisibile
        {
            get => _isProgressVisibile;
            set => SetProperty(ref _isProgressVisibile, value);
        }

        //private ObservableCollection<PostDetailsModel> _lstPostDetailsModel = new ObservableCollection<PostDetailsModel>();

        //public ObservableCollection<PostDetailsModel> LstPostDetailsModel
        //{
        //    get
        //    {
        //        return _lstPostDetailsModel;
        //    }
        //    set
        //    {
        //        if (_lstPostDetailsModel == value)
        //            return;
        //        SetProperty(ref _lstPostDetailsModel, value);
        //    }
        //}

        private ICollectionView _postListsCollectionView;

        public ICollectionView PostListsCollectionView
        {
            get => _postListsCollectionView;
            set
            {
                if (_postListsCollectionView == value)
                    return;
                SetProperty(ref _postListsCollectionView, value);
            }
        }

        #endregion

        #region Create New Post

        public bool CanExecuteCreateNewPost(object sender)
        {
            return true;
        }

        public void ExecuteCreateNewPost(object sender)
        {
            var postDetailsModel = new PostDetailsModel
            {
                CreatedDateTime = DateTime.Now,
                PostDetailsId = Utilities.GetGuid()
            };


            Application.Current.Dispatcher.Invoke(() =>
            {
                PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels.Add(postDetailsModel);
                PostListsCollectionView = CollectionViewSource.GetDefaultView(PublisherCreateCampaigns
                    .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels /*LstPostDetailsModel*/);
                //PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                //    .PublisherCreateCampaignModel.LstPostDetailsModels = LstPostDetailsModel;
            });
        }

        #endregion

        #region Import From Csv

        private bool ImportFromCsvCanExecute(object sender)
        {
            return true;
        }

        private void ImportFromCsvExecute(object sender)
        {
            IsStopLoadingPost = false;

            var listPostDetailsModel = FileUtilities.FileBrowseAndReader();

            var separator = ConstantVariable.Separator;

            if (listPostDetailsModel.Count != 0)
                ThreadFactory.Instance.Start(() =>
                {
                    IsProgressVisibile = Visibility.Visible;
                    IsProgressActive = true;
                    Thread.Sleep(1000);
                    // Iterate selected file name
                    listPostDetailsModel.ForEach(x =>
                    {
                        if (IsStopLoadingPost)
                            return;
                        var postDetailsModel = new PostDetailsModel();
                        try
                        {
                            // Split the file details
                            var allData = x.Split('\t');

                            postDetailsModel.PostDescription = allData[0];

                            // Media list

                            #region Medialist

                            var mediaDetails = allData.Length > 1 ? allData[1] : string.Empty;

                            var mediaUrl = Regex.Split(mediaDetails, separator).ToList();
                            mediaUrl.ForEach(media =>
                            {
                                if (File.Exists(media))
                                    postDetailsModel.MediaViewer.MediaList.Add(media);
                            });

                            #endregion

                            // Title
                            postDetailsModel.PublisherInstagramTitle = allData.Length > 2 ? allData[2] : string.Empty;

                            // Source url
                            postDetailsModel.PdSourceUrl = allData.Length > 3 ? allData[3] : string.Empty;

                            // Facebook Sell post details

                            #region FdSell

                            var FdSellDetails = allData.Length > 4 ? allData[4] : string.Empty;

                            var Fdsell = Regex.Split(FdSellDetails, separator);

                            if (string.Compare(Fdsell[0], "Yes", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                                string.Compare(Fdsell[0], "Y", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                                string.Compare(Fdsell[0], "True", StringComparison.CurrentCultureIgnoreCase) == 0)
                            {
                                postDetailsModel.IsFdSellPost = true;
                                postDetailsModel.FdSellProductTitle = Fdsell[1];
                                postDetailsModel.FdSellPrice = double.Parse(Fdsell[2]);
                                postDetailsModel.FdSellLocation = Fdsell[3];
                                postDetailsModel.FdCondition = Fdsell[4];
                            }

                            #endregion

                            // Created date
                            postDetailsModel.CreatedDateTime = DateTime.Now;

                            // Post id
                            postDetailsModel.PostDetailsId = Utilities.GetGuid();

                            // Add to Collections
                            //postDetails.Add(postDetailsModel);
                            Application.Current.Dispatcher.Invoke(() => PublisherCreateCampaigns
                                .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                                .PublisherCreateCampaignModel.LstPostDetailsModels.Add(postDetailsModel));
                            Thread.Sleep(50);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });
                    IsProgressVisibile = Visibility.Collapsed;
                    IsProgressActive = false;
                    IsStopLoadingPost = false;
                });
        }

        #endregion

        #region Delete Post

        private bool DeletePostCanExecute(object sender)
        {
            return true;
        }

        private void DeletePostExecute(object sender)
        {
            try
            {
                var content = sender as string;
                if (content == "DeleteAll")
                {
                    IsStopLoadingPost = true;
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                            .PublisherCreateCampaignModel.LstPostDetailsModels.Clear();
                        IsStopLoadingPost = false;
                    }));
                }
                else
                {
                    try
                    {
                        var postToDelete = sender as PostDetailsModel;
                        PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                            .PublisherCreateCampaignModel.LstPostDetailsModels.Remove(postToDelete);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    PostListsCollectionView = CollectionViewSource.GetDefaultView(PublisherCreateCampaigns
                        .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                        .PublisherCreateCampaignModel.LstPostDetailsModels)));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}