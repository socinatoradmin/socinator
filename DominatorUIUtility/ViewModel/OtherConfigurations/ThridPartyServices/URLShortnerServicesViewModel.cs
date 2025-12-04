using System.Windows;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class UrlShortnerServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public UrlShortnerServicesViewModel(IGenericFileManager genericFileManager) : base("LangKeyUrlShortnerServices",
            "UrlShortnerServicesControlTemplate")
        {
            _genericFileManager = genericFileManager;
            UrlShortnerServicesModel =
                _genericFileManager.GetModel<UrlShortnerServicesModel>(ConstantVariable.GetURLShortnerServicesFile()) ??
                new UrlShortnerServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        public UrlShortnerServicesModel UrlShortnerServicesModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_genericFileManager.Save(UrlShortnerServicesModel, ConstantVariable.GetURLShortnerServicesFile()))
            {
                ConstantVariable.BitlyLogin = UrlShortnerServicesModel.Login;
                ConstantVariable.BitlyApiKey = UrlShortnerServicesModel.ApiKey;

                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeySuccess".FromResourceDictionary(),
                    "LangKeyUrlShortnerSaved".FromResourceDictionary());
            }
        }
    }
}