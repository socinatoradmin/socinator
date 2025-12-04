using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using QuoraDominatorCore.QdLibrary.QdFunctions;
using System.Threading;
using Unity;

namespace QuoraDominatorCore.QDFactories
{
    public interface IQDBrowserManagerFactory
    {
        IQuoraBrowserManager _QdBrowserManager { get; set; }
        bool CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token, LoginType loginType = LoginType.AutomationLogin);
        IQuoraBrowserManager QdBrowserManager();
    }
    public class QDBrowserManagerFactory : IQDBrowserManagerFactory
    {
        private IUnityContainer unityContainer;
        public QDBrowserManagerFactory(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
        }

        public IQuoraBrowserManager _QdBrowserManager { get ; set; }

        public bool CheckStatusAsync(DominatorAccountModel accountModel, CancellationToken token, LoginType loginType = LoginType.AutomationLogin)
        {
            _QdBrowserManager = unityContainer.Resolve<IQuoraBrowserManager>();
            return _QdBrowserManager.BrowserLoginAsync(accountModel, token,loginType).Result;
        }

        public IQuoraBrowserManager QdBrowserManager()
        {
            return _QdBrowserManager;
        }
    }
}
