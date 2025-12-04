#region

using CommonServiceLocator;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public abstract class BaseTabViewModel : BindableBase, ITabViewModel
    {
        public string Title { get; }
        public string TemplateName { get; }

        protected BaseTabViewModel(string titleResourceName, string templateName)
        {
            var serviceProvider = InstanceProvider.GetInstance<IApplicationResourceProvider>();
            Title = serviceProvider.GetStringResource(titleResourceName);
            TemplateName = templateName;
        }
    }
}