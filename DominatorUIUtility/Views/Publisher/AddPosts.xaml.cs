using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.Views.Publisher.AdvancedOptions;
using Microsoft.Win32;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for AddPosts.xaml
    /// </summary>
    public partial class AddPosts : UserControl, INotifyPropertyChanged
    {
        private static AddPosts ObjAddPosts;
        private AddPostViewModel _addPostViewModel;

        private AddPosts()
        {
            InitializeComponent();
            ObjAddPosts = this;
            SetDataContext();
        }

        public AddPostViewModel AddPostViewModel
        {
            get => _addPostViewModel;
            set
            {
                _addPostViewModel = value;
                OnPropertyChanged(nameof(AddPostViewModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetDataContext()
        {
            AddPostViewModel = new AddPostViewModel();
            AddPostViewModel.AddPostModel.CampaignDetails.LstStatus = new ObservableCollection<string>
            {
                FindResource("LangKeyAll").ToString(),
                FindResource("LangKeyDraft").ToString(),
                FindResource("LangKeyPending").ToString(),
                FindResource("LangKeyPublished").ToString()
            };

            var postDetails = PostFileManager.GetAllPost();
            postDetails.ForEach(x =>
            {
                AddPostViewModel.AddPostModel.CampaignDetails.LstCampaign.Add(new Campaign
                    {CampaignName = x.CampaignDetails.CampaignName});
            });
            MainGrid.DataContext = AddPostViewModel.AddPostModel;

            var ObjCampaigns = Campaigns.GetSingltonCreateCampaignObject();

            ObjCampaigns.createCampign.DataContext = AddPostViewModel.AddPostModel.CampaignDetails;

            var ObjPublisher = Home.GetSingletonHome();
            ObjPublisher.PublisherDetailCollection = CollectionViewSource.GetDefaultView(postDetails);
            ObjPublisher.publisherDetail.ItemsSource = ObjPublisher.PublisherDetailCollection;
        }

        private void btnAddToDrafts_Click(object sender, RoutedEventArgs e)
        {
            var ObjCampaigns = Campaigns.GetSingltonCreateCampaignObject();

            //  AddPostViewModel.AddPostModel.SerialNo = ProtoBuffBase.DeserializeObjects<AddPostModel>(PostDetailFilePath).Count + 1;
            AddPostViewModel.AddPostModel.SerialNo = PostFileManager.GetAllPost().Count + 1;
            AddPostViewModel.AddPostModel.Status = AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus;
            //  PostFileManager.SavePost(AddPostViewModel.AddPostModel);

            // ProtoBuffBase.SerializeObjects(AddPostViewModel.AddPostModel, PostDetailFilePath);
            //manageDraft.Visibility = Visibility.Visible;
            //Campaigns.Visibility = Visibility.Collapsed;
        }

        public static AddPosts GetSingeltonAddPosts()
        {
            if (ObjAddPosts == null)
                ObjAddPosts = new AddPosts();
            return ObjAddPosts;
        }

        /// <summary>
        ///     OnPropertyChanged is used to notify that some property are changed
        /// </summary>
        /// <param name="propertyName">property name</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Jpg images|*.jpg|Png images|*.png|All files (*.*)|*.*"
            };
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return;

            AddMediaSourcesToList(openFileDialog, AddPostViewModel.AddPostModel.PhotoUrl);
            AddPostViewModel.AddPostModel.ImageCount = AddPostViewModel.AddPostModel.PhotoUrl.Count;
        }

        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Mp4 videos|*.mp4|Avi videos|*.avi|All files (*.*)|*.*"
            };
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return;
            AddMediaSourcesToList(openFileDialog, AddPostViewModel.AddPostModel.VideoUrl);

            AddPostViewModel.AddPostModel.VideoCount = AddPostViewModel.AddPostModel.VideoUrl.Count;
        }

        protected void AddMediaSourcesToList(OpenFileDialog openFileDialog, List<string> sourceUrl)
        {
            foreach (var fileName in openFileDialog.FileNames)
            {
                if (sourceUrl.Contains(fileName))
                    continue;
                sourceUrl.Add(fileName);
                AddPostViewModel.AddPostModel.LstMediaSources.Add(fileName);
            }
        }

        private void btnImportPostFromCsv_Click(object sender, RoutedEventArgs e)
        {
            FileUtilities.FileBrowseAndReader().ForEach(x =>
                AddPostViewModel.AddPostModel.ImportedText = AddPostViewModel.AddPostModel.ImportedText + "\r\n" + x);
        }

        private void btnAddToPostList_Click(object sender, RoutedEventArgs e)
        {
            var ObjCampaigns = Campaigns.GetSingltonCreateCampaignObject();
            var portDetails = PostFileManager.GetAllPost();
            int PendingCount;
            int DraftCount;
            int PublishedCount;
            if (!string.IsNullOrEmpty(ObjCampaigns.cmbCampaign.Text) && ObjCampaigns.cmbCampaign.Text ==
                AddPostViewModel.AddPostModel.CampaignDetails.CampaignName)
            {
                var postToupdate = portDetails.FirstOrDefault(post =>
                    post.CampaignDetails.CampaignName == AddPostViewModel.AddPostModel.CampaignDetails.CampaignName);
                PendingCount = postToupdate.PostStatus.PendingCount;
                DraftCount = postToupdate.PostStatus.DraftCount;
                PublishedCount = postToupdate.PostStatus.PublishedCount;


                if (AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus ==
                    FindResource("LangKeyAll").ToString())
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount = PendingCount + 1;

                    AddPostViewModel.AddPostModel.PostStatus.DraftCount = DraftCount + 1;

                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount = PublishedCount + 1;
                }
                else
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Pending"
                            ? PendingCount + 1
                            : PendingCount;

                    AddPostViewModel.AddPostModel.PostStatus.DraftCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Draft"
                            ? DraftCount + 1
                            : DraftCount;

                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Published"
                            ? PublishedCount + 1
                            : PublishedCount;
                }

                AddPostViewModel.AddPostModel.CampaignDetails.SelectedAccount =
                    ObjCampaigns.publishersHeader.cmbAccounts.SelectedValue.ToString();
                PostFileManager.EditPost(AddPostViewModel.AddPostModel);
            }
            else
            {
                AddPostViewModel.AddPostModel.CampaignDetails.SelectedAccount =
                    ObjCampaigns.publishersHeader.cmbAccounts.SelectedValue.ToString();
                AddPostViewModel.AddPostModel.CampaignDetails.CampaignCreatedDate = DateTime.Now;
                AddPostViewModel.AddPostModel.SerialNo = portDetails.Count + 1;


                if (AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus ==
                    FindResource("LangKeyAll").ToString())
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount = 1;

                    AddPostViewModel.AddPostModel.PostStatus.DraftCount = 1;

                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount = 1;
                }
                else
                {
                    AddPostViewModel.AddPostModel.PostStatus.PendingCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Pending" ? 1 : 0;
                    AddPostViewModel.AddPostModel.PostStatus.DraftCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Draft" ? 1 : 0;
                    AddPostViewModel.AddPostModel.PostStatus.PublishedCount =
                        AddPostViewModel.AddPostModel.CampaignDetails.SelectedStatus == "Published" ? 1 : 0;
                }

                AddPostViewModel.AddPostModel.CampaignDetails.LstCampaign.Add(
                    new Campaign {CampaignName = AddPostViewModel.AddPostModel.CampaignDetails.CampaignName});

                PostFileManager.SavePost(AddPostViewModel.AddPostModel);
            }

            SetDataContext();
        }


        private void btnInstagramAdvancedOptions_Click(object sender, RoutedEventArgs e)
        {
            var objInstagramAdvancedOptions = new InstagramAdvancedOptions();
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(objInstagramAdvancedOptions, "Instagram Advanced Options");
            window.Show();
        }

        private void btnAdvancedOptions_Click(object sender, RoutedEventArgs e)
        {
            var objAdvancedOption = new AdvancedOption();
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(objAdvancedOption, "Advanced Options");
            window.Show();
        }

        private void MenuDeleteSingle_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedUrl = ((FrameworkElement) sender).DataContext as string;
            if (AddPostViewModel.AddPostModel.VideoUrl.Contains(selectedUrl))
            {
                AddPostViewModel.AddPostModel.VideoUrl.Remove(selectedUrl);
                AddPostViewModel.AddPostModel.VideoCount -= 1;
            }
            else
            {
                AddPostViewModel.AddPostModel.PhotoUrl.Remove(selectedUrl);
                AddPostViewModel.AddPostModel.ImageCount -= 1;
            }

            AddPostViewModel.AddPostModel.LstMediaSources.Remove(selectedUrl);
        }
    }
}