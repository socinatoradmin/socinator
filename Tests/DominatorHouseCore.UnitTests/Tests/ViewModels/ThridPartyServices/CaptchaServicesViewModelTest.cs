using Dominator.Tests.Utils;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.OtherConfigurations.ThridPartyServices;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels.ThridPartyServices
{
    [TestClass]
    public class CaptchaServicesViewModelTest : UnityInitializationTests
    {
        CaptchaServicesViewModel captchaServicesViewModel;
        private IGenericFileManager _genericFileManager;
        IApplicationResourceProvider _applicationResourceProvider;
    
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _genericFileManager = Substitute.For<IGenericFileManager>();
            _applicationResourceProvider = Substitute.For<IApplicationResourceProvider>();
                     
            Container.RegisterInstance(_applicationResourceProvider);
            Container.RegisterInstance(_genericFileManager);
            captchaServicesViewModel = new CaptchaServicesViewModel(_genericFileManager);
        }
        [TestMethod]
        public void SaveCmd_should_save_CaptchaServices()
        {
            captchaServicesViewModel.SaveCmd.Execute();
            _genericFileManager.Received(1).GetModel<CaptchaServicesModel>(ConstantVariable.GetCaptchaServicesFile());
            _genericFileManager.Received(1).Save(captchaServicesViewModel.CaptchaServicesModel, ConstantVariable.GetCaptchaServicesFile());
        }
    }
}
