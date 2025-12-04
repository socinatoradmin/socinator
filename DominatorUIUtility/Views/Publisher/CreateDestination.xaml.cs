using System.Windows;
using System.Windows.Controls;
using DominatorUIUtility.Behaviours;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for CreateDestination.xaml
    /// </summary>
    public partial class CreateDestination : UserControl
    {
        public CreateDestination()
        {
            InitializeComponent();
        }

        private void OpenContextMenu_OnClick(object sender, RoutedEventArgs e)
        {
            ViewUtilites.OpenContextMenu(sender);
        }

        private void BtnBackToCampaign_Click(object sender, RoutedEventArgs e)
        {
            ManageDestinationIndex.Instance.SelectedControl = new ManageDestination();
        }
    }
}