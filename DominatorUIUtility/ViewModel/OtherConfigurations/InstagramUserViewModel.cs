using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class InstagramUserViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public InstagramUserViewModel(IGenericFileManager genericFileManager) : base("LangKeyInstagram",
            "InstagramControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            InstagramUserModel =
                _genericFileManager.GetModel<InstagramUserModel>(ConstantVariable.GetOtherInstagramSettingsFile()) ??
                new InstagramUserModel();
        }

        public InstagramUserModel InstagramUserModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_genericFileManager.Overrride(InstagramUserModel, ConstantVariable.GetOtherInstagramSettingsFile()))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyInstaConfigurationSaved".FromResourceDictionary());
        }
    }
}