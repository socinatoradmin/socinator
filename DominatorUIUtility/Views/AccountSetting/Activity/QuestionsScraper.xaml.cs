using System.Windows.Controls;
using DominatorUIUtility.ViewModel.Startup.ModuleConfig;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    ///     Interaction logic for QuestionsScraper.xaml
    /// </summary>
    public partial class QuestionsScraper : UserControl
    {
        public QuestionsScraper(IQuestionsScraperViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}