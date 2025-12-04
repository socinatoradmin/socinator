using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.PuppeteerBrowser;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows;

namespace DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices
{
    public class FunCaptchaServiceViewModel : BaseTabViewModel, IThridPartyServicesViewModel
    {
        public FunCaptchaServiceModel FunCaptchaServiceModel { get; }
        private readonly IPuppeteerBrowserManager browserManager;
        private JsonHandler handler => JsonHandler.GetInstance;
        public DelegateCommand SaveCmd { get; }
        public DelegateCommand CheckBalance { get; }
        public DelegateCommand ChooseFolder { get; }
        private readonly IGenericFileManager _genericFileManager;
        public FunCaptchaServiceViewModel(IGenericFileManager genericFileManager, IPuppeteerBrowserManager puppeteerBrowser) : base("LangKeyFunCaptchaService", "FunCaptchaServices")
        {
            _genericFileManager = genericFileManager;
            browserManager = puppeteerBrowser;
            FunCaptchaServiceModel =
                _genericFileManager.GetModel<FunCaptchaServiceModel>(ConstantVariable
                    .GetFunCaptchaServicesFile()) ?? new FunCaptchaServiceModel();
            SaveCmd = new DelegateCommand(Save);
            CheckBalance = new DelegateCommand(UpdateBalance);
            ChooseFolder = new DelegateCommand(ChooseFolderExecute);
        }

        private void ChooseFolderExecute()
        {
            try
            {
                var downloadedFolderPath = FileUtilities.GetExportPath();
                if (string.IsNullOrEmpty(downloadedFolderPath))
                    downloadedFolderPath = FunCaptchaServiceModel.CaptchaSettingPath ?? ConstantVariable.GetCapSolverExtension();
                else
                {
                    DirectoryUtilities.DeleteFolder(new System.Collections.Generic.List<string> { FunCaptchaServiceModel.CaptchaSettingPath },true);
                    downloadedFolderPath = downloadedFolderPath.EndsWith("CapSolver") ? downloadedFolderPath : downloadedFolderPath + "\\CapSolver";
                }
                DirectoryUtilities.CreateDirectory(downloadedFolderPath);
                FunCaptchaServiceModel.CaptchaSettingPath = downloadedFolderPath;
            }
            catch { }
        }

        private void Save()
        {
            if(FunCaptchaServiceModel != null)
            {
                if (string.IsNullOrEmpty(FunCaptchaServiceModel.APIKey))
                {
                    Dialog.ShowDialog("Warning","Please Provide Fun Captcha API Key");
                    return;
                }
                else
                {
                    try
                    {
                        UpdateBalance();
                    }
                    catch { }
                    finally{
                        if (_genericFileManager.Save(FunCaptchaServiceModel, ConstantVariable.GetFunCaptchaServicesFile()))
                        {
                            try
                            {
                                ExtractExtension();
                            }
                            catch { }
                            Application.Current.Dispatcher.Invoke(async () =>
                            {
                                var result = Dialog.ShowCustomDialog("LangKeySuccess".FromResourceDictionary(),
                                    "Fun Captcha Key Saved Successfully.To Apply Setting Please Restart The Socinator!",
                                    "LangKeyRestartNow".FromResourceDictionary(),
                                    "LangKeyRestartLater".FromResourceDictionary());
                                if (result == MessageDialogResult.Affirmative)
                                {
                                    await browserManager.RemoveAllBrowser();
                                    Application.Current.Shutdown();
                                    Process.Start(Application.ResourceAssembly.Location);
                                    Process.GetCurrentProcess().Kill();
                                    Environment.Exit(0);
                                }
                            });
                        }
                    }
                }
            }
        }
        public bool ExtractExtension()
        {
            try
            {
                var Destination = FunCaptchaServiceModel.CaptchaSettingPath ?? ConstantVariable.GetCapSolverExtension();
                DirectoryUtilities.CreateDirectory(Destination);
                var info = new DirectoryInfo(Destination);
                if (info != null && info.GetFiles("*.*")?.Length == 0)
                {
                    var source = $"{Environment.CurrentDirectory}\\CapSolver.zip";
                    if (File.Exists(source))
                    {
                        ZipFile.ExtractToDirectory(source, Destination);
                    }
                }
                return true;
            }
            catch { return false; }
        }
        private void UpdateBalance()
        {
            if(FunCaptchaServiceModel != null)
            {
                if (string.IsNullOrEmpty(FunCaptchaServiceModel.APIKey))
                {
                    Dialog.ShowDialog("Warning", "Please Provide Fun Captcha API Key");
                    return;
                }
                else
                {
                    try
                    {
                        var Response = HttpHelper.PostResponseStreamAsync("https://api.capsolver.com/getBalance",Encoding.UTF8.GetBytes($"{{\"clientKey\": \"{FunCaptchaServiceModel.APIKey}\"}}")).Result;
                        var jObject = handler.ParseJsonToJsonObject(Response?.Response);
                        var balance = handler.GetJTokenValue(jObject, "balance");
                        var errorId = handler.GetJTokenValue(jObject, "errorId");
                        var arrayObject = handler.GetJTokenOfJToken(jObject, "packages")?.FirstOrDefault();
                        FunCaptchaServiceModel.Balance = string.IsNullOrEmpty(balance) ? string.IsNullOrEmpty(FunCaptchaServiceModel.Balance) ? "0.0 $": FunCaptchaServiceModel.Balance : $"{balance} $";
                        if(arrayObject != null && arrayObject.HasValues)
                        {
                            FunCaptchaServiceModel.Details = new FunCaptchaDetails
                            {
                                Balance = balance,
                                ErrorId = errorId,
                                PackageId = handler.GetJTokenValue(arrayObject, "packageId"),
                                Type = handler.GetJTokenValue(arrayObject, "type"),
                                NumberOfCalls = handler.GetJTokenValue(arrayObject, "numberOfCalls"),
                                Status = handler.GetJTokenValue(arrayObject, "status"),
                                CaptchaToken = handler.GetJTokenValue(arrayObject, "token"),
                                ActiveTime = handler.GetJTokenValue(arrayObject, "activeTime"),
                                ExpireTime = handler.GetJTokenValue(arrayObject, "expireTime")
                            };
                        }
                        else
                        {
                            FunCaptchaServiceModel.Details = new FunCaptchaDetails
                            {
                                ErrorId = errorId,
                                CaptchaToken = FunCaptchaServiceModel.APIKey
                            };
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
