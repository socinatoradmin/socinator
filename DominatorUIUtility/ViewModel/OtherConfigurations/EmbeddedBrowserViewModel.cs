using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class EmbeddedBrowserViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IOtherConfigFileManager _otherConfigFileManager;

        public EmbeddedBrowserViewModel(IOtherConfigFileManager otherConfigFileManager) : base(
            "LangKeyEmbeddedBrowserSettings", "EmbeddedBrowserControlTemplate")
        {
            _otherConfigFileManager = otherConfigFileManager;
            SaveCmd = new DelegateCommand(Save);
            EmbeddedBrowserModel = _otherConfigFileManager.GetOtherConfig<EmbeddedBrowserSettingsModel>() ??
                                   new EmbeddedBrowserSettingsModel();
        }

        public EmbeddedBrowserSettingsModel EmbeddedBrowserModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_otherConfigFileManager.SaveOtherConfig(EmbeddedBrowserModel))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyUrlBrowserSettingsSaved".FromResourceDictionary());
        }
    }
}