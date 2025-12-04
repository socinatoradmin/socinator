using CommonServiceLocator;
using DominatorHouse.ThreadUtils;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using Unity.ServiceLocation;

namespace Dominator.Tests.Utils
{
    [TestClass]
    public class UnityInitializationTests
    {
        protected IUnityContainer Container;
        protected IDelayService DelayService;

        [TestInitialize]
        public virtual void SetUp()
        {
            DelayService = Substitute.For<IDelayService>();
            Container = new UnityContainer();
            Container.RegisterInstance<IDateProvider>(Substitute.For<IDateProvider>());
            Container.RegisterInstance<IDelayService>(DelayService);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));
        }
    }
}
