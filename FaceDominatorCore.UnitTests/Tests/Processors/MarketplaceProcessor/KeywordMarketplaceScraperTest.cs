using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDLibrary.FdProcessors.MarketplaceProcessor;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.FilterModel;
using FaceDominatorCore.FDResponse.AccountsResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Processors.MarketplaceProcessor
{
    [TestClass]
    public class KeywordMarketplaceScraperTest : BaseFacebookProcessorTest
    {
        //private KeywordMarketplaceScraper _sut;
        //private IFdJobProcess _fdJobProcess;
        //private IDbGlobalService _dbGlobalService;
        //private IGlobalInteractionDetails _globalInteractionDetails;
        //private ITemplatesFileManager _templatesFileManager;

        //[TestInitialize]
        //public override void SetUp()
        //{
        //    base.SetUp();

        //    _fdJobProcess = Substitute.For<IFdJobProcess>();
        //    Container.RegisterInstance(_fdJobProcess);
        //    _dbGlobalService = Substitute.For<IDbGlobalService>();
        //    _globalInteractionDetails = Substitute.For<IGlobalInteractionDetails>();
        //    Container.RegisterInstance(_globalInteractionDetails);
        //    _templatesFileManager = Substitute.For<ITemplatesFileManager>();
        //    Container.RegisterInstance(_templatesFileManager);
        //    var moduleSetting = new ModuleSetting();
        //    _fdJobProcess.ModuleSetting.Returns(moduleSetting);

        //    ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
        //}

        //[TestMethod]
        //public void CustomUserChannelProcessor_Should_Execute_Successfully()
        //{
        //    var cancellationTokenSource = new System.Threading.CancellationTokenSource();
        //    _fdJobProcess.JobCancellationTokenSource.Returns(cancellationTokenSource);

        //    var marketplaceFilterModel = new MarketplaceFilterModel();
        //    ProcessScopeModel.GetActivitySettingsAs<MarketplaceFilterModel>().Returns(marketplaceFilterModel);

        //    _fdJobProcess = new CommentLikerProcesss(ProcessScopeModel, DbAccountServiceScoped, GlobalService, ExecutionLimitsManager,
        //        FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped);

        //    FdRequestLibrary.GetAccountMarketPlaceDetails(Arg.Any<DominatorAccountModel>());

        //    var queryInfo = new QueryInfo { QueryValue = "" };
        //    _fdJobProcess.FinalProcess(new ScrapeResultNew());

        //    FdLibraryTest.LikeComments_Should_Return_True(FdHttpHelper);

        //    _sut = new KeywordMarketplaceScraper
        //        (_fdJobProcess, DbAccountServiceScoped, DbCampaignServiceScoped, _dbGlobalService, FdRequestLibrary, ProcessScopeModel);

        //    // act
        //    _sut.Start(queryInfo);

        //    // assert
        //    FdRequestLibrary.Received(1).GetPostCommentor(Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), null);
        //    FdLibraryTest.Received(1).LikeComments_Should_Return_True(FdHttpHelper);
        //    //_fdJobProcess.Received(1).FinalProcess(Arg.Any<ScrapeResultNew>());
        //}
    }
}
