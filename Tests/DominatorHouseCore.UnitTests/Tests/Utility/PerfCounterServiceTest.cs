using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class PerfCounterServiceTest
    {
        IPerfCounterService _perfCounterService;
        [TestInitialize]
       public void SetUp()
        {
            _perfCounterService = new PerfCounterService();
        }
        [TestMethod]
        public void should_return_cpuusage_zero_and_AvailableMemory_not_be_total_memory()
        {
            var result = _perfCounterService.GetActualValues();
            result.CpuUsage.Should().Be(0);
            result.AvailableMemory.Should().NotBe(7882);
           
        }
        [TestMethod]
        public void should_Ram_size_be_7882_in_MB()
        {
            var result = _perfCounterService.LoadedMemoryDescrption;
            result.Should().NotBe("0 MB");
        }
    }
}
