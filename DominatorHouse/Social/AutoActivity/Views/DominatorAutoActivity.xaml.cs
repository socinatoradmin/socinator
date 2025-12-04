using DominatorHouse.Social.AutoActivity.ViewModels;

namespace Socinator.Social.AutoActivity.Views
{
    /// <summary>
    /// Interaction logic for DominatorAutoActivity.xaml
    /// </summary>
    public partial class DominatorAutoActivity
    {
        public DominatorAutoActivity(IDominatorAutoActivityViewModel activityViewModel)
        {
            InitializeComponent();
            DataContext = activityViewModel;
        }

    }
}
