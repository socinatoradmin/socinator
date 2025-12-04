using System;
using System.IO;
using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using EmbeddedBrowser;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDLibrary.GrowConnectionProcesses;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Scraper;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;

namespace LinkedDominatorCore.LDLibrary.ScraperProcesses
{
    public class MessageConversationProcess : LDJobProcessInteracted<
        InteractedUsers>
    {
        private readonly ILdFunctions _ldFunctions;
        private readonly IProcessScopeModel _processScopeModel;
        private BrowserAutomationExtension _automationExtension;


        public MessageConversationProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory,
            ILdHttpHelper ldHttpHelper, ILdLogInProcess logInProcess,
            ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory,
                ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            MessageConversationScraperModel =
                processScopeModel.GetActivitySettingsAs<MessageConversationScraperModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _processScopeModel = processScopeModel;
        }

        public MessageConversationScraperModel MessageConversationScraperModel { get; set; }
        public BrowserWindow BrowserWindow { get; set; }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var detailedUserInfoJasonString = string.Empty;
            var success = true;
            var attachmentFileName = "";
            var attachmentFileUrl = "";
            var fileName = "";
            var i = 0;

            var jobProcessResult = new JobProcessResult();
            try
            {
                var linkedinUser = (LinkedinUser) scrapeResult.ResultUser;
                try
                {
                    if (linkedinUser.FileNameAndUrls.Count != 0)
                    {
                        var downloadPath = "";
                        if (MessageConversationScraperModel.IsDownloadAllAttachmetinOneFolder)
                            downloadPath =
                                Utils.GetMessageFilePath(_processScopeModel.CampaignDetails.CampaignName, "");
                        else
                            downloadPath = Utils.GetMessageFilePath(_processScopeModel.CampaignDetails.CampaignName,
                                DominatorAccountModel.AccountBaseModel.UserFullName);
                        var files = "";
                        foreach (var userAttachment in linkedinUser.FileNameAndUrls)
                        {
                            files += (attachmentFileName = userAttachment.Key==null? DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") : userAttachment.Key).Trim() + "\n";
                            attachmentFileUrl = userAttachment.Value;

                            GlobusLogHelper.log.Info(Log.StartedActivity,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, "Download or scrap attachment",
                                attachmentFileName ?? linkedinUser.Username);
                            if (MessageConversationScraperModel.IsDonwloadAttachments)
                            {
                                if (IsBrowser && !string.IsNullOrEmpty(attachmentFileUrl))
                                {
                                    var name = LDAccountsBrowserDetails.GetBrowserName(DominatorAccountModel,
                                        BrowserInstanceType.Primary);
                                    BrowserWindow = LDAccountsBrowserDetails.GetInstance()
                                        .AccountBrowserCollections[name];
                                    _automationExtension = new BrowserAutomationExtension(BrowserWindow);
                                    var MessageButtonClass = LDClassesConstant.Messenger.MessageButtonClass;
                                    var ClickIndex = 1;
                                    var nodes = HtmlAgilityHelper.GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(BrowserWindow.GetPageSource(), "", true, MessageButtonClass);
                                    var IsMyConnection = !nodes.Any(x => x.Contains("Connect") || x.Contains("Follow"));
                                    if (!IsMyConnection)
                                    {
                                        MessageButtonClass = MessageButtonClass?.Replace("primary", "secondary");
                                        ClickIndex = 4;
                                    }
                                    var isClickedmessage =
                                        _automationExtension.ExecuteScript(AttributeIdentifierType.ClassName,
                                            MessageButtonClass,1,ClickIndex,EventType.click);
                                    if (!isClickedmessage.Success)
                                        isClickedmessage = _automationExtension.ExecuteScript(
                                            AttributeIdentifierType.ClassName,
                                            MessageButtonClass,1,ClickIndex,EventType.click);
                                    if (!isClickedmessage.Success)
                                    {
                                        isClickedmessage =
                                            _automationExtension.ExecuteScript(AttributeIdentifierType.Xpath,
                                                "//span[text()='More…']");
                                        isClickedmessage = _automationExtension.ExecuteScript(
                                            AttributeIdentifierType.ClassName,
                                            "message-anywhere-button link-without-visted-state t-black--light t-normal pv-s-profile-actions__label display-flex");
                                    }
                                    var extension = attachmentFileName.Contains(".")
                                        ? attachmentFileName.Split('.').ToList().LastOrDefault()
                                        : string.Empty;
                                    if (string.IsNullOrEmpty(extension))
                                    {
                                        var getExtensionFromRemoteUrl = Utilities.GetFileExtensonFromRemoteUrl(attachmentFileUrl, _ldFunctions.GetInnerHttpHelper().Request.CookieContainer);
                                        if (!string.IsNullOrEmpty(getExtensionFromRemoteUrl) && !getExtensionFromRemoteUrl.Contains("Error"))
                                            extension = getExtensionFromRemoteUrl;
                                    }
                                    var directory =
                                        $@"{downloadPath}\{linkedinUser.Username}-{DominatorAccountModel.AccountBaseModel.UserFullName}";
                                    while (true)
                                    {
                                        var isDirectoryExist = Directory.Exists(directory);
                                        if (isDirectoryExist)
                                        {
                                            ++i;
                                            directory =
                                                $@"{downloadPath}\{linkedinUser.Username} {i}-{DominatorAccountModel.AccountBaseModel.UserFullName}";
                                        }
                                        else if (i == 0)
                                        {
                                            directory =
                                                $@"{downloadPath}\{linkedinUser.Username}-{DominatorAccountModel.AccountBaseModel.UserFullName}";
                                            break;
                                        }
                                        else
                                        {
                                            directory =
                                                $@"{downloadPath}\{linkedinUser.Username} {i}-{DominatorAccountModel.AccountBaseModel.UserFullName}";
                                            break;
                                        }
                                    }
                                    try
                                    {
                                        if (!Directory.Exists(directory))
                                            Directory.CreateDirectory(directory);
                                        fileName = $"{directory}.{extension}";
                                    }
                                    catch (Exception) { }
                                    var downloadHandler = new DownloadHandler(BrowserWindow);
                                    BrowserWindow.Browser.DownloadHandler = downloadHandler;
                                    downloadHandler.SuggestedName = fileName;
                                    var query = string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToClick, "", "download",attachmentFileName.Trim(),0);
                                    var isClicked = _automationExtension.ExecuteScript(query, 10);
                                    if (!isClicked.Success)
                                    {
                                        isClicked.Success = _ldFunctions.GetInnerLdHttpHelper().DownloadFile(attachmentFileUrl, fileName);
                                    }
                                    if (isClicked.Success)
                                        GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                            $"to Path [ {fileName} ]");
                                    else
                                        GlobusLogHelper.log.Info(Log.ActivityFailed,
                                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                            DominatorAccountModel.AccountBaseModel.UserName, $"to Path [ {fileName} ]",
                                            linkedinUser.Username, "");
                                }
                                else
                                {
                                    var extension = attachmentFileName.Contains(".")
                                        ? attachmentFileName.Split('.').ToList().LastOrDefault()
                                        : string.Empty;
                                    if (string.IsNullOrEmpty(extension))
                                    {
                                        var getExtensionFromRemoteUrl = Utilities.GetFileExtensonFromRemoteUrl(attachmentFileUrl, _ldFunctions.GetInnerHttpHelper().Request.CookieContainer);
                                        if (!string.IsNullOrEmpty(getExtensionFromRemoteUrl) && !getExtensionFromRemoteUrl.Contains("Error"))
                                            extension = getExtensionFromRemoteUrl;
                                    }
                                    var directory =
                                        $@"{downloadPath}\{linkedinUser.Username}-{DominatorAccountModel.AccountBaseModel.UserFullName}";

                                    while (true)
                                    {
                                        var isDirectoryExist = Directory.Exists(directory);
                                        if (isDirectoryExist)
                                        {
                                            ++i;
                                            directory =
                                                $@"{downloadPath}\{linkedinUser.Username} {i}-{DominatorAccountModel.AccountBaseModel.UserFullName}";
                                        }
                                        else if (i == 0)
                                        {
                                            directory =
                                                $@"{downloadPath}\{linkedinUser.Username}-{DominatorAccountModel.AccountBaseModel.UserFullName}";
                                            break;
                                        }
                                        else
                                        {
                                            directory =
                                                $@"{downloadPath}\{linkedinUser.Username} {i}-{DominatorAccountModel.AccountBaseModel.UserFullName}";
                                            break;
                                        }
                                    }
                                    try
                                    {
                                        if (!Directory.Exists(directory))
                                            Directory.CreateDirectory(directory);
                                        fileName = $"{directory}.{extension}";
                                    }
                                    catch (Exception) { }
                                    if (!string.IsNullOrEmpty(attachmentFileUrl))
                                    {
                                        if (_ldFunctions.GetInnerLdHttpHelper()
                                            .DownloadFile(attachmentFileUrl, fileName))
                                            GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                                $"to Path [ {fileName} ]");
                                        else
                                            GlobusLogHelper.log.Info(Log.ActivityFailed,
                                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                DominatorAccountModel.AccountBaseModel.UserName,
                                                $"to Path [ {fileName} ]", linkedinUser.Username, "");
                                    }
                                }
                            }
                        }

                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        var profileUrl = $"https://www.linkedin.com/in/{linkedinUser.PublicIdentifier}";
                        linkedinUser.AccountUserFullName =
                            DominatorAccountModel.AccountBaseModel.UserFullName;
                        linkedinUser.AccountUserProfileUrl =
                            DominatorAccountModel.AccountBaseModel.ProfilePictureUrl;
                        linkedinUser.FullName = linkedinUser.Username;
                        linkedinUser.ProfileId = linkedinUser.ProfileId;
                        linkedinUser.ProfileUrl = profileUrl;
                        detailedUserInfoJasonString = string.Join("\n", files, downloadPath);
                        linkedinUser.AttachmentId = linkedinUser.attachmentDetails[0].Value.Trim();
                    }
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException("Operation Cancelled!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                if (success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "scrap message details from",
                        linkedinUser.Username);
                    IncrementCounters();
                    DbInsertionHelper.UserScraper(scrapeResult, linkedinUser, detailedUserInfoJasonString);
                    jobProcessResult.IsProcessSuceessfull = true;
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "scrap message details from",
                        linkedinUser.Username, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

            return jobProcessResult;
        }
    }
}