using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for WhitelistuserControl.xaml
    /// </summary>
    public partial class PrivateWhitelistUserControl
    {
        public PrivateWhitelistUserControl(IPrivateWhiteListViewModel privateWhiteListViewModel)
        {
            InitializeComponent();
            MainGrid.DataContext = privateWhiteListViewModel;
        }
    }
}