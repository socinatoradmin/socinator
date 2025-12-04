using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using System.Threading;
using Unity;

namespace FaceDominatorCore.FDFactories
{
    public interface IBrowserManagerFactory
    {
        bool CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token,LoginType loginType = LoginType.AutomationLogin);

        IFdBrowserManager FdBrowserManager(DominatorAccountModel accountModel, CancellationToken token);

    }

    public class BrowserManagerFactory : IBrowserManagerFactory
    {

        public static object LockAccountUpdate = new object();
        private IFdBrowserManager _fdBrowserManager;
        private IFdBaseBrowserManger _fdBaseBrowserManger;
        private readonly IUnityContainer unityContainer;
        public BrowserManagerFactory(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public bool CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token, LoginType loginType = LoginType.AutomationLogin)
        {
            _fdBaseBrowserManger = unityContainer.Resolve<IFdBaseBrowserManger>();

            return _fdBaseBrowserManger.BrowserLogin(accountModel, token,loginType:loginType);
        }

        public IFdBrowserManager FdBrowserManager(DominatorAccountModel accountModel, CancellationToken token)
        {

            InstanceProvider.GetInstance<IUnityContainer>()
                .RegisterType<IFdBrowserManager, FdBrowserManager>();
            unityContainer.RegisterInstance<IFdBrowserManager>(new FdBrowserManager());


            _fdBrowserManager = unityContainer.Resolve<IFdBrowserManager>();
            if (_fdBaseBrowserManger == null) _fdBaseBrowserManger = unityContainer.Resolve<IFdBaseBrowserManger>();
            _fdBrowserManager.BrowserWindow = _fdBaseBrowserManger.BrowserWindow;

            return _fdBrowserManager;
        }


    }
}
