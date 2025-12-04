#region

using System.Collections.Concurrent;
using Unity;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface IAccountScopeFactory
    {
        IUnityContainer this[string accountId] { get; }
    }

    public class AccountScopeFactory : IAccountScopeFactory
    {
        private readonly IUnityContainer _unityContainer;
        private readonly ConcurrentDictionary<string, IUnityContainer> _scopes;

        public AccountScopeFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            _scopes = new ConcurrentDictionary<string, IUnityContainer>();
        }


        public IUnityContainer this[string accountId] => _scopes.GetOrAdd(accountId, CreateScope);

        private IUnityContainer CreateScope(string accountId)
        {
            var scope = _unityContainer.CreateChildContainer();
            scope.RegisterInstance("AccountId", accountId);
            return scope;
        }
    }
}