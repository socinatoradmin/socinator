using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for ProxyManager.xaml
    /// </summary>
    public partial class ProxyManager
    {
        public ProxyManager(IProxyManagerViewModel proxyManagerViewModel)
        {
            InitializeComponent();
            MainGrid.DataContext = proxyManagerViewModel;
            proxyManagerViewModel.ProxyDataGrid = ProxyDataGrid;
        }
    }
}