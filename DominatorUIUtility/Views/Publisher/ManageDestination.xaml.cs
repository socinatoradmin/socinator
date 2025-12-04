using System.Windows;
using System.Windows.Controls;
using DominatorUIUtility.Behaviours;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for ManageDestination.xaml
    /// </summary>
    public partial class ManageDestination : UserControl
    {
        public ManageDestination()
        {
            InitializeComponent();
        }

        private void ButtonCreateDestination_OnClick(object sender, RoutedEventArgs e)
        {
            ManageDestinationIndex.Instance.SelectedControl = new CreateDestination();
        }

        private void BtnBackToCampaign_Click(object sender, RoutedEventArgs e)
        {
            PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = Home.GetSingletonHome();
        }

        private void OpenContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            ViewUtilites.OpenContextMenu(sender);
        }
    }
}