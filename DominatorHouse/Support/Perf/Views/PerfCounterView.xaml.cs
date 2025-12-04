using DominatorHouse.ViewModels;

namespace DominatorHouse.Support.Perf.Views
{
    /// <summary>
    /// Interaction logic for PerfCounterView.xaml
    /// </summary>
    public partial class PerfCounterView
    {
        public PerfCounterView(IPerfCounterViewModel perfCounterViewModel)
        {
            InitializeComponent();
            DataContext = perfCounterViewModel;
        }
    }
}
