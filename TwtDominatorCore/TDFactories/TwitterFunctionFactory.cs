using System;
using DominatorHouseCore;
using DominatorHouseCore.Models;
using TwtDominatorCore.TDLibrary;
using Unity;

namespace TwtDominatorCore.TDFactories
{
    public interface ITwitterFunctionFactory
    {
        ITwitterFunctions TwitterFunctions { get; set; }

        void AssignTwitterFunctions(DominatorAccountModel account);
        void AssignHttpTwitterFunctions();
    }

    public class TwitterFunctionFactory : ITwitterFunctionFactory
    {
        private readonly IUnityContainer _unityContainer;

        private bool isCalled;

        public TwitterFunctionFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            // here we assigning it as a default
            AssignHttpTwitterFunctions();
        }

        public ITwitterFunctions TwitterFunctions { get; set; }

        public void AssignTwitterFunctions(DominatorAccountModel account)
        {
            try
            {
                if (isCalled)
                    return;
                TwitterFunctions = account.IsRunProcessThroughBrowser
                    ? _unityContainer.Resolve<ITwitterFunctions>("browser")
                    : _unityContainer.Resolve<ITwitterFunctions>();
                isCalled = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog("Issue in Resolving ITwitterFunctions");
            }
        }

        public void AssignHttpTwitterFunctions()
        {
            TwitterFunctions = _unityContainer.Resolve<ITwitterFunctions>();
        }
    }
}