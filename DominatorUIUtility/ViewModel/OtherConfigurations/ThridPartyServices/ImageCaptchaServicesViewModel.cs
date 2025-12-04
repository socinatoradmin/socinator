using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class ImageCaptchaServicesViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public ImageCaptchaServicesViewModel(IGenericFileManager genericFileManager) : base(
            "LangKeyImageCaptchaServices", "ImageCaptchaServices")
        {
            _genericFileManager = genericFileManager;
            ImageCaptchaServicesModel =
                _genericFileManager.GetModel<ImageCaptchaServicesModel>(ConstantVariable
                    .GetImageCaptchaServicesFile()) ?? new ImageCaptchaServicesModel();
            SaveCmd = new DelegateCommand(Save);
        }

        public ImageCaptchaServicesModel ImageCaptchaServicesModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_genericFileManager.Save(ImageCaptchaServicesModel, ConstantVariable.GetImageCaptchaServicesFile()))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyImageCaptchaSaved".FromResourceDictionary());
        }
    }
}