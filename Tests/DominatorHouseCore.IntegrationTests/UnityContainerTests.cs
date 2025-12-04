using CommonServiceLocator;
using DominatorHouseCore.DatabaseHandler.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.ServiceLocation;

namespace DominatorHouseCore.IntegrationTests
{
    [TestClass]
    public class UnityContainerTests
    {
        private IUnityContainer _sut;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new UnityContainer();
            _sut.AddNewExtension<CoreUnityExtension>();
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(_sut));
        }

        [TestMethod]
        public void should_resolve_all_registered_components()
        {
            foreach (var containerRegistration in _sut.Registrations)
            {
                if (containerRegistration.RegisteredType == typeof(IDbOperations))
                {
                    // skip it because it resolves with string parameter
                    continue;
                }

                if (string.IsNullOrWhiteSpace(containerRegistration.Name))
                {
                    var aService = _sut.Resolve(containerRegistration.RegisteredType);
                    aService.Should().NotBeNull();
                }
                else
                {
                    var aService = _sut.Resolve(containerRegistration.RegisteredType, containerRegistration.Name);
                    aService.Should().NotBeNull();
                }
            }
        }
    }
}
