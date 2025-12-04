using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity;

namespace GramDominatorCore.GDFactories
{
    public interface IBrowserManagerFactory
    {
        IGdBrowserManager _gdBrowserManager { get; set; }
        bool CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token, LoginType loginType = LoginType.AutomationLogin);
        IGdBrowserManager GdBrowserManager(DominatorAccountModel accountModel, CancellationToken token);
    }

    public class BrowserManagerFactory : IBrowserManagerFactory
    {
        public static object LockAccountUpdate = new object();
        public IGdBrowserManager _gdBrowserManager { get; set; }
        private readonly IUnityContainer unityContainer;

        public BrowserManagerFactory(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }
        public bool CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token, LoginType loginType = LoginType.AutomationLogin)
        {
            _gdBrowserManager = unityContainer.Resolve<IGdBrowserManager>();
            return _gdBrowserManager.BrowserLogin(accountModel, token, loginType);
        }

        public IGdBrowserManager GdBrowserManager(DominatorAccountModel accountModel, CancellationToken token)
        {
            //CommonServiceLocator.InstanceProvider.GetInstance<IUnityContainer>().
            //    RegisterType<IGdBrowserManager, GdBrowserManager>();
            //unityContainer.RegisterInstance<IGdBrowserManager>(new GdBrowserManager());

            
            

            return _gdBrowserManager;
        }
    }
}
