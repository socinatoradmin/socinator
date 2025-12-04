using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GramDominatorCore.GDLibrary;
using DominatorHouseCore.Models;
using Unity;
using DominatorHouseCore;

namespace GramDominatorCore.GDFactories
{
    public interface IInstaFunctFactory
    {
        IInstaFunction InstaFunctions { get; set; }

        IInstaFunction AssignInstaFunctions(DominatorAccountModel account);
        void AssignHttpInstaFunctFunctions();
    }

    public class InstaFunctFactory: IInstaFunctFactory
    {
        public IInstaFunction InstaFunctions { get; set; }

        private readonly IUnityContainer _unityContainer;

        public InstaFunctFactory(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
            // here we assigning it as a default
            AssignHttpInstaFunctFunctions();
        }

        //bool isCalled = false;
        public IInstaFunction AssignInstaFunctions(DominatorAccountModel account)
        {
            try
            {
                InstaFunctions = account.IsRunProcessThroughBrowser ? _unityContainer.Resolve<IInstaFunction>() :
                _unityContainer.Resolve<IInstaFunction>();
               // isCalled = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog("Issue in Resolving IInstaFunctions");
            }
            return InstaFunctions;
        }
        public void AssignHttpInstaFunctFunctions()
        {
            InstaFunctions = _unityContainer.Resolve<IInstaFunction>();
        }
    }
}
