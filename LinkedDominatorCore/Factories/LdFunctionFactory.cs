using DominatorHouseCore.Models;
using LinkedDominatorCore.LDLibrary;
using Unity;

namespace LinkedDominatorCore.Factories
{
    public interface ILdFunctionFactory
    {
        ILdFunctions LdFunctions { get; set; }
        void AssignFunction(DominatorAccountModel accountModel);
        void AssignBrowserFunction();
    }

    public class LdFunctionFactory : ILdFunctionFactory
    {
        private readonly IUnityContainer _unityContainer;

        public LdFunctionFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            LdFunctions = _unityContainer.Resolve<ILdFunctions>();
        }

        public ILdFunctions LdFunctions { get; set; }

        public void AssignFunction(DominatorAccountModel accountModel)
        {
            LdFunctions = accountModel.IsRunProcessThroughBrowser
                ? _unityContainer.Resolve<ILdFunctions>("browser")
                : _unityContainer.Resolve<ILdFunctions>();
        }

        public void AssignBrowserFunction()
        {
            LdFunctions = _unityContainer.Resolve<ILdFunctions>("browser");
        }
    }
}