using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    ///     Interaction logic for PublishedPostDetails.xaml
    /// </summary>
    public partial class PublishedPostDetails : UserControl, INotifyPropertyChanged
    {
        private PublishedPostDetailsViewModel _publishedPostDetailsViewModel = new PublishedPostDetailsViewModel();

        public PublishedPostDetails()
        {
            InitializeComponent();
        }

        public PublishedPostDetails(PublisherPostlistModel currentData) : this()
        {
            try
            {
                PublishedPostDetailsViewModel.PublisherPostlist =
                    PostlistFileManager.GetByPostId(currentData.CampaignId, currentData.PostId);
                DataContext = PublishedPostDetailsViewModel;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public PublishedPostDetailsViewModel PublishedPostDetailsViewModel
        {
            get => _publishedPostDetailsViewModel;
            set
            {
                if (_publishedPostDetailsViewModel == value)
                    return;
                _publishedPostDetailsViewModel = value;
                OnPropertyChanged(nameof(PublishedPostDetailsViewModel));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var lb = (ListView) sender;
                var postLink = (lb?.SelectedItem as PublishedPostDetailsModel).Link;
                if (!string.IsNullOrEmpty(postLink))
                {
                    Clipboard.SetText(postLink);
                    ToasterNotification.ShowSuccess("Message copied");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}