#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public interface ITabViewModel : INotifyPropertyChanged
    {
        string Title { get; }
        string TemplateName { get; }
    }
}