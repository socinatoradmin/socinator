namespace DominatorHouseCore.ViewModel.DashboardVms
{
    public interface IDashboardViewModel : ITabViewModel
    {
        string CurrentVersion { get; set; }
    }
}