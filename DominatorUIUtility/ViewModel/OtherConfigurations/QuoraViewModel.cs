using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class QuoraViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public QuoraViewModel(IGenericFileManager genericFileManager) : base("LangKeyQuora", "QuoraControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            QuoraModel = _genericFileManager.GetModel<QuoraModel>(ConstantVariable.GetOtherQuoraSettingsFile()) ??
                         new QuoraModel();
        }

        public QuoraModel QuoraModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_genericFileManager.Overrride(QuoraModel, ConstantVariable.GetOtherQuoraSettingsFile()))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyQuoraConfigurationSaved".FromResourceDictionary());
        }
    }
}