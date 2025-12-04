using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDRequest;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public interface IFdLoginProcess : ILoginProcessAsync
    {
        void RequestParameterInitialized(DominatorAccountModel dominatorAccountModel);

        IFdBaseBrowserManger _BasebrowserManager { get; set; }
        IFdBrowserManager _browserManager { get; set; }

        Task<bool> CheckLoginPostScrapperAsync(DominatorAccountModel dominatorAccountModel,
            CancellationToken cancellationToken, bool isPublihsherModule = false);

    }

    public class FdLoginProcess : IFdLoginProcess
    {
        //After adding multiple accounts maintaing browser count
        public static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(5, 5);
        private readonly IBrowserManagerFactory _browserManagerFactory;
        private readonly IFdHttpHelper _httpHelper;
        private readonly IFdRequestLibrary _fdRequestLibrary;
        public IFdBaseBrowserManger _BasebrowserManager { get; set; }
        public IFdBrowserManager _browserManager { get; set; }
        public LoginType loginType1 { get; set; }
        public FdLoginProcess(IFdHttpHelper httpHelper,
            IFdRequestLibrary requestLibrary, IFdBrowserManager browserManager,
            IBrowserManagerFactory browserManagerFactory, IFdBaseBrowserManger fdBaseBrowserManger)
        {
            _httpHelper = httpHelper;
            _fdRequestLibrary = requestLibrary;
            _BasebrowserManager = fdBaseBrowserManger;
            _browserManager = browserManager;
            _browserManagerFactory = browserManagerFactory;
        }

        bool ILoginProcess.CheckLogin(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken)
            => CheckLoginAsync(dominatorAccountModel, cancellationToken).Result;

        public async Task<bool> CheckLoginAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token, bool displayLoginMsg = false, LoginType loginType = LoginType.AutomationLogin)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                loginType1 = loginType;
                GlobusLogHelper.log.Info(Log.AccountLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);

                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    return await FacebookWebLogin(dominatorAccountModel, token);
                else
                {
                    await LoginWithAlternativeMethodAsync(dominatorAccountModel, token);
                    return dominatorAccountModel.IsUserLoggedIn;
                }

            }
            catch (Exception)
            {
                semaphoreSlim.Release();
            }
            finally
            {
                if (dominatorAccountModel.IsUserLoggedIn)
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        dominatorAccountModel.AccountBaseModel.UserName);
                if (loginType1 != LoginType.UpdateDetails && dominatorAccountModel.IsRunProcessThroughBrowser && (dominatorAccountModel.AccountBaseModel.NeedToCloseBrowser || !dominatorAccountModel.IsUserLoggedIn))
                {
                    _browserManager = _browserManagerFactory.FdBrowserManager(dominatorAccountModel, token);
                    _browserManager.CloseBrowser(dominatorAccountModel);
                }
                semaphoreSlim.Release();
            }
            return false;

        }


        public async Task<bool> CheckLoginPostScrapperAsync(DominatorAccountModel dominatorAccountModel
            , CancellationToken token, bool isPublihsherModule = false)
        {
            if (dominatorAccountModel.IsRunProcessThroughBrowser && isPublihsherModule)
            {
                await LoginWithAlternativeMethodAsync(dominatorAccountModel, token);
                _browserManager = _browserManagerFactory.FdBrowserManager(dominatorAccountModel, token);
                _browserManager.CloseBrowser(dominatorAccountModel);
                return dominatorAccountModel.IsUserLoggedIn;
            }
            else
                return await FacebookWebLoginPostScraper(dominatorAccountModel, token, isPublihsherModule);

        }

        public void LoginWithDataBaseCookies(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken token)
        {
            try
            {
                RequestParameterInitialized(dominatorAccountModel);
                _fdRequestLibrary.Login(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }


        public async Task LoginWithDataBaseCookiesAsync(DominatorAccountModel dominatorAccountModel, bool isMobileRequired, CancellationToken cancellationToken)
            => await FacebookWebLogin(dominatorAccountModel, cancellationToken);




        public async Task<bool> FacebookWebLogin(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            try
            {
                RequestParameterInitialized(dominatorAccountModel);

                var isLoggedIn = await _fdRequestLibrary.LoginAsync(dominatorAccountModel, token);

                if (isLoggedIn)
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);

                return isLoggedIn;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }


        public void LoginWithAlternativeMethod(DominatorAccountModel dominatorAccountModel, CancellationToken token)
            => _BasebrowserManager.BrowserLogin(dominatorAccountModel, dominatorAccountModel.Token);


        public async Task LoginWithAlternativeMethodAsync(DominatorAccountModel dominatorAccountModel, CancellationToken token)
        {
            RequestParameterInitialized(dominatorAccountModel);
            await Task.Run(() => _browserManagerFactory.CheckStatusAsync(dominatorAccountModel, token,loginType:loginType1));
        }



        public async Task<bool> FacebookWebLoginPostScraper(DominatorAccountModel dominatorAccountModel
            , CancellationToken token, bool isPublihsherModule)
        {
            try
            {
                RequestParameterInitialized(dominatorAccountModel);

                return isPublihsherModule ? await _fdRequestLibrary.LoginAsync(dominatorAccountModel, token) :
                     await _fdRequestLibrary.LoginForPostScrapperAsync(dominatorAccountModel, token);
            }
            catch (Exception)
            {
                Console.WriteLine();
            }
            return false;
        }

        // replace it later with non static class method "RequestParameterInitialized"
        public static void RequestParameterInitialize(DominatorAccountModel dominatorAccountModel)
        {

            var _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            IHttpHelper _httpHelper = _accountScopeFactory[dominatorAccountModel.AccountId].Resolve<IFdHttpHelper>();

            dominatorAccountModel.UserAgentWeb = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            dominatorAccountModel.UserAgentWeb = string.IsNullOrEmpty(dominatorAccountModel.UserAgentWeb) ? GetRandomUseragent() : dominatorAccountModel.UserAgentWeb;
            try
            {
                var fdRequestParameter =
                    new FdRequestParameter
                    {
                        Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy,
                        Cookies = dominatorAccountModel.Cookies ?? new CookieCollection(),
                        UserAgent = dominatorAccountModel.UserAgentWeb
                    };

                IRequestParameters parameter = fdRequestParameter;

                _httpHelper.SetRequestParameter(parameter);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        public void RequestParameterInitialized(DominatorAccountModel dominatorAccountModel)
        {
            dominatorAccountModel.UserAgentWeb = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
            dominatorAccountModel.UserAgentWeb = string.IsNullOrEmpty(dominatorAccountModel.UserAgentWeb) ? GetRandomUseragent() : dominatorAccountModel.UserAgentWeb;
            try
            {
                var fdRequestParameter =
                    new FdRequestParameter
                    {
                        Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy,
                        Cookies = dominatorAccountModel.Cookies ?? new CookieCollection(),
                        UserAgent = dominatorAccountModel.UserAgentWeb
                    };

                IRequestParameters parameter = fdRequestParameter;

                _httpHelper.SetRequestParameter(parameter);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        public static void RequestParameterInitializeHttpHelper(DominatorAccountModel dominatorAccountModel, ref FdHttpHelper httpHelper)
        {
            var userAgentWeb = string.IsNullOrEmpty(dominatorAccountModel.UserAgentWeb) ? GetRandomUseragent() : dominatorAccountModel.UserAgentWeb;
            try
            {
                var fdRequestParameter =
                    new FdRequestParameter
                    {
                        Proxy = dominatorAccountModel.AccountBaseModel.AccountProxy,
                        UserAgent = userAgentWeb
                    };
                httpHelper.SetRequestParameter(fdRequestParameter);


            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        private static string GetRandomUseragent()
        {
            return new[]
            {
                // collection of user agent
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36"


            }.GetRandomItem();
        }

        public void LoginWithBrowserMethod(DominatorAccountModel dominatorAccountModel, CancellationToken token, VerificationType verificationType = 0,
            LoginType loginType = LoginType.AutomationLogin)
        {
            if (_browserManagerFactory.CheckStatusAsync(dominatorAccountModel, token))
                RequestParameterInitialized(dominatorAccountModel);

            _browserManager = _browserManagerFactory.FdBrowserManager(dominatorAccountModel, token);
        }

    }
}
