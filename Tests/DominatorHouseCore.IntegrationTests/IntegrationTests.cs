using CommonServiceLocator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.ServiceLocation;

namespace DominatorHouseCore.IntegrationTests
{
    public class IntegrationTests<T>
    {
        protected IUnityContainer Container;
        protected T Sut;

        [TestInitialize]
        public virtual void SetUp()
        {
            Container = new UnityContainer();
            Container.AddNewExtension<CoreUnityExtension>();
            OverrideDependencies(Container);
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(Container));

            Sut = Container.Resolve<T>();
        }

        protected virtual void OverrideDependencies(IUnityContainer container)
        {

        }
    }
}
