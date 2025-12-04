using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for FanpageLiker.xaml
    /// </summary>
    public partial class FanpageLiker : UserControl
    {
        public FanpageLiker(IFanpageLikerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}