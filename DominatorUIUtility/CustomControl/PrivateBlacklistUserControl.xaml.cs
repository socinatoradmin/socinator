using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for BlacklistUserControl.xaml
    /// </summary>
    public partial class PrivateBlacklistUserControl
    {
        public PrivateBlacklistUserControl(IPrivateBlickListViewModel privateBlickListViewModel)
        {
            InitializeComponent();
            MainGrid.DataContext = privateBlickListViewModel;
        }
    }
}