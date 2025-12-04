using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class CaptchaServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public CaptchaServicesViewModel(IGenericFileManager genericFileManager) : base("LangKeyCaptchaServices",
            "CaptchaServicesControlTemplate")
        {
            _genericFileManager = genericFileManager;
            CaptchaServicesModel =
                _genericFileManager.GetModel<CaptchaServicesModel>(ConstantVariable.GetCaptchaServicesFile()) ??
                new CaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        public CaptchaServicesModel CaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            var Message = "Please Provide {0} API Key Then Save The Settings.";
            if(CaptchaServicesModel != null && !CaptchaServicesModel.IsTwoCaptcha && !CaptchaServicesModel.IsAntiCaptcha) {
                Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                    "Please Select Captcha Service And Provide API Key To Save The Settings.");
                return;
            }
            else if(CaptchaServicesModel != null && CaptchaServicesModel.IsTwoCaptcha && string.IsNullOrEmpty(CaptchaServicesModel.TwoCaptchaApiKey)) 
            {
                Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                    string.Format(Message,"Two Captcha"));
                return;
            }
            else if (CaptchaServicesModel != null && CaptchaServicesModel.IsAntiCaptcha && string.IsNullOrEmpty(CaptchaServicesModel.AntiCaptchaApiKey))
            {
                Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                    string.Format(Message, "Anti Captcha"));
                return;
            }else if (_genericFileManager.Save(CaptchaServicesModel, ConstantVariable.GetCaptchaServicesFile()))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyCaptchaSaved".FromResourceDictionary());
        }
    }
}