using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for MessageToPlaces.xaml
    /// </summary>
    public partial class MessageToPlaces : UserControl
    {
        public MessageToPlaces(IMessageToPlacesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}