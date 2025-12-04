using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using DominatorHouse.ViewModels;
using System.Windows;
using Dominator.Tests.Utils;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels
{
    [TestClass]
    public class PerfCounterViewModelTest : UnityInitializationTests
    {
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            Container.RegisterSingleton<IPerfCounterViewModel, PerfCounterViewModel>();
            Container.RegisterSingleton<IPerfCounterService, PerfCounterService>();
        }
        [TestMethod]
        public void LoadedMemory_should_7882_mb_and_LogViewHeight_shold_3_star()
        {
            var PerfCounterViewModel = Container.Resolve<PerfCounterViewModel>();
            PerfCounterViewModel.LoadedMemory.Should().NotBe("0 MB");
            PerfCounterViewModel.LogViewHeight.Should().Be(new GridLength(3, GridUnitType.Star));
            PerfCounterViewModel.ShowHideLogCmd.Should().NotBeNull();
        }
    }
   
}
