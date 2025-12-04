using DominatorHouseCore.Models;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for CustomizeAutoActivity.xaml
    /// </summary>
    public partial class CustomizeAutoActivity
    {
        public NetworksActivityCustomizeViewModel ViewModel;

        public CustomizeAutoActivity(NetworksActivityCustomizeModel model)
        {
            ViewModel = new NetworksActivityCustomizeViewModel(model);
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}