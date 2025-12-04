using FaceDominatorCore.FDLibrary.FdProcesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaceDominatorCore.FDModel.CommonSettings;
using NSubstitute;
using DominatorHouseCore.Models;
using Dominator.Tests.Utils;
using DominatorHouseCore.Request;
using System.Reflection;
using FaceDominatorCore.FDLibrary.FdProcessors.EventProcessors;

namespace FaceDominatorCore.UnitTests.Tests.Processors.EventProcessors
{
    [TestClass]
    public class EventCreatorProcessorsTest:BaseFacebookProcessorTest
    {
        private EventCreatorProcessors _sut;
        private IFdJobProcess _fdJobProcess;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
        }

        [TestMethod]
        public void EventCreatorProcessors_Should_Execute_Successfully()
        {
           
        }
    }
}
