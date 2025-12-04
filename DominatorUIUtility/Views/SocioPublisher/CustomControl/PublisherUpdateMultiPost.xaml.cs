using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    ///     Interaction logic for PublisherUpdateMultiPost.xaml
    /// </summary>
    public partial class PublisherUpdateMultiPost : UserControl
    {
        public PublisherUpdateMultiPost()
        {
            InitializeComponent();
        }

        public PublisherUpdateMultiPost(List<PublisherPostlistModel> selectedPost) : this()
        {
            LstPostDetail = new ObservableCollection<PublisherPostlistModel>(selectedPost);
            MainGrid.DataContext = this;
        }

        public ObservableCollection<PublisherPostlistModel> LstPostDetail { get; set; }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                PostlistFileManager.UpdatePostlists(LstPostDetail.FirstOrDefault().CampaignId, LstPostDetail);
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