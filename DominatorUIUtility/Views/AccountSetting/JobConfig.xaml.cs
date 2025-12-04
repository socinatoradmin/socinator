using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.AccountSetting
{
    /// <summary>
    ///     Interaction logic for JobConfig.xaml
    /// </summary>
    public partial class JobConfig
    {
        public JobConfig(IJobConfigViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}