using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class PinterestOtherConfigViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public PinterestOtherConfigViewModel(IGenericFileManager genericFileManager) : base("LangKeyPinterest",
            "PinterestControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            PinterestOtherConfigModel =
                _genericFileManager.GetModel<PinterestOtherConfigModel>(
                    ConstantVariable.GetOtherPinterestSettingsFile()) ??
                new PinterestOtherConfigModel();
        }

        public PinterestOtherConfigModel PinterestOtherConfigModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_genericFileManager.Overrride(PinterestOtherConfigModel,
                ConstantVariable.GetOtherPinterestSettingsFile()))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyPinterestConfigurationSaved".FromResourceDictionary());
        }
    }
}