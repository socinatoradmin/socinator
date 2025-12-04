using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for Campaigns.xaml
    /// </summary>
    public partial class Campaigns : UserControl
    {
        private static Campaigns _objCreateCampaign;

        public Campaigns()
        {
            InitializeComponent();
            publishersHeader.HeaderText = FindResource("LangKeyCreateCampaign").ToString();
            CreateCampaignTabs.ItemsSource = InitializeTabs();
            _objCreateCampaign = this;
        }

        private IEnumerable<TabItemTemplates> InitializeTabs()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyAddPosts").ToString(),
                    Content = new Lazy<UserControl>(AddPosts.GetSingeltonAddPosts)
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyClickableImagePost").ToString()
                    //   Content=new Lazy<UserControl>(()=>new AddPosts())
                },
                new TabItemTemplates
                {
                    Title = FindResource("LangKeySharePost").ToString()
                    // Content=new Lazy<UserControl>(()=>new AddPosts())
                }
            };
            return tabItems;
        }

        public static Campaigns GetSingltonCreateCampaignObject()
        {
            return _objCreateCampaign ?? (_objCreateCampaign = new Campaigns());
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            ObjAddPosts.manageDraft.Visibility = Visibility.Collapsed;
            ObjAddPosts.createCampaign.Visibility = Visibility.Visible;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     OnPropertyChanged is used to notify that some property are changed
        /// </summary>
        /// <param name="propertyName">property name</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void cmbCampaign_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            var selectedCampaign = string.Empty;
            if (cmbCampaign.SelectedItem != null)
                selectedCampaign = (cmbCampaign.SelectedItem as Campaign).CampaignName;
            if (!string.IsNullOrEmpty(selectedCampaign))
            {
                ObjAddPosts.AddPostViewModel.AddPostModel.CampaignDetails.CampaignName = selectedCampaign;
                ObjAddPosts.MainGrid.DataContext = PostFileManager.GetAllPost()
                    .FirstOrDefault(post => post.CampaignDetails.CampaignName == selectedCampaign);
            }
            else
            {
                ObjAddPosts.SetDataContext();
            }
        }


        private void tglStatus_IsCheckedChanged(object sender, EventArgs e)
        {
            var ObjAddPosts = AddPosts.GetSingeltonAddPosts();
            var OnLabel = tglStatus.OnLabel;
            switch (OnLabel)
            {
                case "Completed":
                    ObjAddPosts.AddPostViewModel.AddPostModel.CampaignDetails.Status = "Stopped";
                    break;
                case "Stopped":
                    ObjAddPosts.AddPostViewModel.AddPostModel.CampaignDetails.Status = "Active";
                    break;
                case "Active":
                    ObjAddPosts.AddPostViewModel.AddPostModel.CampaignDetails.Status = "Paused";
                    break;
                case "Paused":
                    ObjAddPosts.AddPostViewModel.AddPostModel.CampaignDetails.Status = "Completed";
                    break;
            }
        }
    }
}