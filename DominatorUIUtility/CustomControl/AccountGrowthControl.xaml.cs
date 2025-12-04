using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for AccountGrowthControl.xaml
    /// </summary>
    public partial class AccountGrowthControl
    {
        public AccountGrowthControl(IAccountGrowthControlViewModel accountGrowthControlViewModel)
        {
            InitializeComponent();
            DataContext = accountGrowthControlViewModel;
        }
    }
}