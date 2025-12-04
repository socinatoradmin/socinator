using System.ComponentModel;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.Startup;
using Prism.Regions;

namespace DominatorUIUtility
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ModuleSetting
    {
        private static ModuleSetting _instance;
        private readonly IRegionManager _regionManager;

        public ModuleSetting(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            SetValue(RegionManager.RegionManagerProperty, regionManager);
        }

        public static ModuleSetting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ModuleSetting(InstanceProvider.GetInstance<IRegionManager>());
                var viewModel = InstanceProvider.GetInstance<ISelectActivityViewModel>();
                _instance.Title =
                    $"{"LangKeySocinator".FromResourceDictionary()} - {{ {viewModel.SelectedNetwork} }} ( {viewModel.SelectAccount.UserName} )";
                return _instance;
            }
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Instance.Visibility = Visibility.Collapsed;
            StartupBaseViewModel.selectedIndex = 0;
            StartupBaseViewModel.ViewModelToSave.Clear();
            _regionManager.Regions["StartupRegion"].RemoveAll();
            _regionManager.RequestNavigate("StartupRegion", "SelectActivity");
        }
    }
}