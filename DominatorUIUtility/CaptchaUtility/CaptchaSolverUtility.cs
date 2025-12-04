using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace DominatorUIUtility.CaptchaUtility
{
    public class CaptchaSolverUtility
    {
        public FunCaptchaServiceModel funCaptcha { get; set; }
        public FunCaptchaServiceViewModel viewModel { get; set; }
        private IGenericFileManager genericFileManager;
        public PuppeteerBrowserActivity PuppeteerBrowser { get; set; }
        public virtual bool InitializeProperties()
        {
            try
            {
                genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                viewModel = InstanceProvider.GetInstance<IThridPartyServicesViewModel>("FunCaptchaServiceViewModel") as FunCaptchaServiceViewModel;
                funCaptcha = viewModel.FunCaptchaServiceModel ?? genericFileManager.GetModel<FunCaptchaServiceModel>(ConstantVariable.GetFunCaptchaServicesFile()) ?? new FunCaptchaServiceModel();
                return true;
            }
            catch { return false; }
        }
        public virtual async Task ApplyKeyBoardShortCutToCaptchaExtension(DominatorAccountModel dominatorAccount)
        {
        }
        public virtual bool IsBalanceAvailable(DominatorAccountModel dominatorAccount)
        {
            try
            {
                InitializeProperties();
                double.TryParse(funCaptcha?.Balance?.Replace("$", "")?.Trim(), out double balance);
                if (balance <= 0)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                                dominatorAccount.AccountBaseModel.UserName, "Captcha Solve",
                                "You don't have sufficient balance to solve captcha. Please renew your captcha pack to solve captcha.Thanks!");
                    return false;
                }
                return true;
            }
            catch { return false; }
        }
        public virtual async Task<bool> LaunchPuppeteerBrowser(DominatorAccountModel dominatorAccount,bool IsHeadLess=true,string TargetPageUrl = "https://chromewebstore.google.com/search/Capsolver",List<string> ExtensionsCollections=null)
        {
            PuppeteerBrowser = new PuppeteerBrowserActivity(dominatorAccount, isNeedResourceData: true);
            try
            {
                return await PuppeteerBrowser.LaunchBrowserAsync(IsHeadLess, TargetPageUrl, loadExtension: true,ExtensionCollection:ExtensionsCollections);
            }
            catch { return false; }
        }
        public virtual async Task<bool> SolveCaptcha(DominatorAccountModel dominatorAccount)
        {
            return false;
        }
        public virtual async Task<bool> ConfigureCaptchaExtension(DominatorAccountModel dominatorAccount)
        {
            return false;
        }
    }
}
