using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDModel.Profilling;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Request;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using ThreadUtils;

namespace LinkedDominatorCore.LDLibrary
{
    public class ProfileEndorsementProcess : LDJobProcessInteracted<DominatorHouseCore.DatabaseHandler.LdTables.Account.InteractedUsers>
    {
        public ProfileEndorsementModel ProfileEndorsementModel { get; set; }
        private string CurrentActivityType { get; set; }
        private readonly ILdFunctions _ldFunctions;
        private readonly IDelayService _delayService;
        public string campiagnid = string.Empty;

        public ProfileEndorsementProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IDbGlobalService globalService,
            IExecutionLimitsManager executionLimitsManager, ILdQueryScraperFactory queryScraperFactory, ILdHttpHelper ldHttpHelper,
            ILdLogInProcess logInProcess, ILdFunctionFactory ldFunctionFactory, IDbInsertionHelper dbInsertionHelper)
            : base(processScopeModel, accountServiceScoped, globalService, executionLimitsManager, queryScraperFactory, ldHttpHelper, logInProcess, dbInsertionHelper)
        {
            ProfileEndorsementModel = processScopeModel.GetActivitySettingsAs<ProfileEndorsementModel>();
            _ldFunctions = ldFunctionFactory.LdFunctions;
            _delayService = InstanceProvider.GetInstance<IDelayService>();
            CurrentActivityType = ActivityType.ProfileEndorsement.ToString();
        }


        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            #region MyRegion

            var jobProcessResult = new JobProcessResult();
            try
            {
                #region Other Variables Initializations

                var profileId = string.Empty;

                #endregion

                var objLinkedinUser = (LinkedinUser) scrapeResult.ResultUser;
                var isUserFilterActive =
                    LdUserFilterProcess.IsUserFilterActive(ProfileEndorsementModel.LDUserFilterModel);

                #region Filters After Visiting Profile

                try
                {
                    if (isUserFilterActive)
                    {
                        var isValidUser = LdUserFilterProcess.GetFilterStatus(objLinkedinUser.ProfileUrl,
                            ProfileEndorsementModel.LDUserFilterModel, _ldFunctions);
                        if (!isValidUser)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                "[ " + objLinkedinUser.FullName + " ] is not a valid user according to the filter.");
                            jobProcessResult.IsProcessSuceessfull = false;
                            return jobProcessResult;
                        }
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "endorse skills for connection",
                    objLinkedinUser.FullName);

                var endorsedSkillsCollection = IsBrowser
                    ? BrowserEndorsement(objLinkedinUser, jobProcessResult, isUserFilterActive)
                    : NormalEndorsement(objLinkedinUser, profileId, jobProcessResult);

                endorsedSkillsCollection = Utils.InsertSpecialCharactersInCsv(endorsedSkillsCollection).Trim(':');
                if(!string.IsNullOrEmpty(endorsedSkillsCollection) && endorsedSkillsCollection.Contains($"No Skills Found for {objLinkedinUser.FullName}"))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,DominatorAccountModel.AccountBaseModel.AccountNetwork,DominatorAccountModel.AccountBaseModel.UserName, ActivityType,endorsedSkillsCollection);
                }
                else if (!string.IsNullOrEmpty(endorsedSkillsCollection))
                {
                    IncrementCounters();
                    DbInsertionHelper.ProfileEndorsement(scrapeResult, objLinkedinUser, endorsedSkillsCollection);
                }

                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Operation Cancelled!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            #endregion

            return jobProcessResult;
        }

        private string BrowserEndorsement(LinkedinUser objLinkedinUser, JobProcessResult jobProcessResult,
            bool isUserFilterActive)
        {
            var automationExtension = new BrowserAutomationExtension(_ldFunctions.BrowserWindow);
            if (!isUserFilterActive)
                automationExtension.LoadPageUrlAndWait(objLinkedinUser.ProfileUrl);
            //automationExtension.ScrollWindow(10000, true, "LinkedIn Corporation ©");
            var publicIdentifier = objLinkedinUser.PublicIdentifier;
            if (string.IsNullOrEmpty(publicIdentifier))
                publicIdentifier = Utils.GetBetween(objLinkedinUser.ProfileUrl + "**", "in/", "**");
            automationExtension.LoadPageUrlAndWait(LdConstants.UserActivityURL(publicIdentifier,"skills"));
            var pageSource = _ldFunctions.BrowserWindow.GetPageSource();
            if (!string.IsNullOrEmpty(pageSource) && pageSource.Contains("Nothing to see for now"))
                return $"No Skills Found for {objLinkedinUser.FullName}";
            var skillsList =
                HtmlAgilityHelper.GetListNodesFromAttibute(pageSource, HTMLTags.Div, AttributeIdentifierType.componentkey, null, "com.linkedin.sdui.profile.skill");
            skillsList.RemoveAll(x =>!string.IsNullOrEmpty(x.InnerHtml) && (x.InnerHtml.Contains("-divider")||x.InnerHtml.Contains("Endorsed")));
            var endorsedSkills = "";
            var skillsEndorsed = 0;

            if (skillsList.Count == 0)
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
            var List = new List<string>();
            foreach (var htmlNode in skillsList)
            {
                if (skillsEndorsed >= ProfileEndorsementModel.NumberOfSkillsToBeEndorsed)
                {
                    List.Clear();
                    break;
                }
                
                var skillName = HtmlAgilityHelper.GetListInnerTextFromTagName(htmlNode.OuterHtml, "p")?.FirstOrDefault();
                if (List.Contains(skillName))
                    continue;
                string endorsementResponse = null;
                var EndorseString = HtmlAgilityHelper.GetListInnerTextFromTagName(htmlNode.InnerHtml,"span")?.FirstOrDefault(x=>x == "Endorse");
                if (string.IsNullOrEmpty(EndorseString))
                    EndorseString = HtmlAgilityHelper.GetListInnerTextFromTagName(htmlNode.InnerHtml, "span")?.FirstOrDefault(x => x == "Endorsed");
                if (!string.IsNullOrEmpty(EndorseString))
                {
                    if (string.Equals(EndorseString, "Endorse"))
                    {
                        _delayService.ThreadSleep(2500);
                        var Script = string.Format(LDClassesConstant.ScriptConstant.ScriptWithQuerySelectorAllToFilterNormal, $"{HTMLTags.Div} {HTMLTags.Button} {HTMLTags.Span}", "Endorse",0, "scrollIntoViewIfNeeded();");
                        automationExtension.ExecuteScript(Script);
                        var endorsed = automationExtension.ExecuteScript(Script.Replace("scrollIntoViewIfNeeded();", "click();"), 5).Success;
                        if(endorsed)
                        {
                            endorsementResponse = "";
                            List.Add(skillName);
                        }
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                            DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            DominatorAccountModel.AccountBaseModel.UserName,
                            "endorse skill [" + skillName + "] for connection", "you have already endorsed this skill");
                        skillsEndorsed++;
                        continue;
                    }
                } 
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                EndorsedSkillsCollection(objLinkedinUser, jobProcessResult, endorsementResponse, skillName, ref endorsedSkills, ref skillsEndorsed);
                _delayService.ThreadSleep(5000);
            }


            return endorsedSkills;
        }

        private string NormalEndorsement(LinkedinUser objLinkedinUser, string profileId,
            JobProcessResult jobProcessResult)
        {
            string actionUrl;
            string endorsementResponse;
            var endorsedSkillsCollection = string.Empty;
            #region PostString
            profileId = !string.IsNullOrEmpty(objLinkedinUser.ProfileId) ? objLinkedinUser.ProfileId : string.Empty;
            var lstSkills = GetSkills(objLinkedinUser.ProfileUrl, objLinkedinUser.FullName, ref profileId, objLinkedinUser.TrackingId);



            if (lstSkills.Count != 0)
            {
                var skillsEndorsed = 0;

                foreach (var skill in lstSkills)
                {
                    try
                    {
                        if (skillsEndorsed >= ProfileEndorsementModel.NumberOfSkillsToBeEndorsed)
                            break;

                        var skillName = skill.Split(':')[0];
                        var skillId = skill.Split(':')[1];

                        actionUrl = "https://www.linkedin.com/voyager/api/identity/profiles/" + profileId + "/normEndorsements";
                        var postData = "{\"skill\":{\"name\":\"" + skillName + "\",\"entityUrn\":\"urn:li:fs_skill:(" +
                                       profileId + "," + skillId + ")\"}}";

                        #region Endorse Skill

                        endorsementResponse = _ldFunctions.GetInnerLdHttpHelper().HandlePostResponse(actionUrl, postData)
                            ?.Response;
                        if(endorsementResponse == null)
                            endorsementResponse = _ldFunctions.GetInnerLdHttpHelper().HandlePostResponse(actionUrl, postData)
                            ?.Response;
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        EndorsedSkillsCollection(objLinkedinUser, jobProcessResult, endorsementResponse, skillName, ref endorsedSkillsCollection, ref skillsEndorsed);
                        #endregion

                        _delayService.ThreadSleep(5000);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException("Operation Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }


            }
            else
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
            }

            #endregion

            return endorsedSkillsCollection;
        }

        private void EndorsedSkillsCollection(LinkedinUser objLinkedinUser, JobProcessResult jobProcessResult,
            string endorsementResponse, string skillName, ref string endorsedSkillsCollection, ref int skillsEndorsed)
        {
            if (endorsementResponse == "")
            {
                GlobusLogHelper.log.Info(Log.ActivitySuccessful,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName,
                    "endorse skill [" + skillName + "] for connection", objLinkedinUser.FullName);
                jobProcessResult.IsProcessSuceessfull = true;
                endorsedSkillsCollection += skillName + ":";
                skillsEndorsed++;
            }
            else
            {
                
                GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName,
                    "endorse skill [" + skillName + "] for connection", $"{objLinkedinUser.FullName},sorry! endorsing option not available or you have already endorsed this skill");
                jobProcessResult.IsProcessSuceessfull = false;
            }
        }


        public List<string> GetSkills(string profileUrl, string fullName, ref string profileId, string trackingID = "")
        {
            var lstSkills = new List<string>();
            var skillsResponse = string.Empty;

            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "getting skills for " + fullName + "");
                var cookies = new CookieContainer();
                var csrf = string.Empty;
                var handler = new HttpClientHandler
                {
                    CookieContainer = cookies,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                foreach(Cookie cookie in DominatorAccountModel.Cookies)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(csrf) && cookie.Name == "JSESSIONID")
                            csrf = cookie?.Value?.Replace("\"","");
                        cookies.Add(new Cookie { Name = cookie.Name, Value = cookie.Value, Domain = cookie.Domain });
                    }
                    catch (Exception)
                    {
                    }
                }
                using (var client = new HttpClient(handler))
                {
                    // 🔹 Set request headers
                    client.DefaultRequestHeaders.Add("Host", "www.linkedin.com");
                    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                    client.DefaultRequestHeaders.Add("X-UDID", "a22fb207-87f7-4752-8c3c-3db3c7b624dd");
                    client.DefaultRequestHeaders.Add("X-RestLi-Protocol-Version", "2.0.0");
                    client.DefaultRequestHeaders.Add("x-li-graphql-pegasus-client", "true");
                    client.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                    client.DefaultRequestHeaders.Add("Csrf-Token", csrf);
                    if(!string.IsNullOrEmpty(trackingID))
                        client.DefaultRequestHeaders.Add("X-li-page-instance", $"urn:li:page:p_flagship3_profile_view_base_skills_details;{trackingID}");
                    client.DefaultRequestHeaders.Add("X-LI-Track", "{\"osName\":\"Android OS\",\"osVersion\":\"31\",\"clientVersion\":\"4.1.980\",\"clientMinorVersion\":188800,\"model\":\"vivo_V2029\",\"displayDensity\":2,\"displayWidth\":720,\"displayHeight\":1554,\"dpi\":\"xhdpi\",\"deviceType\":\"android\",\"appId\":\"com.linkedin.android\",\"deviceId\":\"a22fb207-87f7-4752-8c3c-3db3c7b624dd\",\"timezoneOffset\":5.5,\"timezone\":\"Asia/Kolkata\",\"storeId\":\"us_googleplay\",\"isAdTrackingLimited\":false,\"mpName\":\"voyager-android\",\"mpVersion\":\"1.58.145\"}");
                    client.DefaultRequestHeaders.Add("X-LI-PEM-Metadata", "Voyager - Profile=view-skills-details");
                    client.DefaultRequestHeaders.Add("x-restli-symbol-table-name", "voyager-20141");
                    client.DefaultRequestHeaders.Add("X-LI-Lang", "en_US");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "com.linkedin.android/188800 (Linux; U; Android 12; en_US; V2029; Build/SP1A.210812.003; Cronet/127.0.6533.65)");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");

                    // 🔹 LinkedIn GraphQL URL
                    var url = $"https://www.linkedin.com/voyager/api/graphql?variables=(sectionType:skills,profileUrn:urn%3Ali%3Afsd_profile%3A{profileId})&queryName=ProfileComponentsBySectionType&queryId=voyagerIdentityDashProfileComponents.7ff07e4961ec91ab4e660839161f1ad8";

                    try
                    {
                        var response = client.GetAsync(url).Result;
                        var body = response.Content.ReadAsStringAsync().Result;
                        var parser = JsonJArrayHandler.GetInstance;
                        var obj = parser.ParseJsonToJObject(body);
                        var skillToken = parser.GetJTokenOfJToken(obj, "data", "identityDashProfileComponentsBySectionType", "elements",0, "components", "tabComponent", "sections", 0, "subComponent", "components", "pagedListComponent", "components");
                        var PaginationToken = parser.GetJTokenOfJToken(skillToken, "paging");
                        var SkillsCollection = parser.GetJArrayElement(parser.GetJTokenValue(skillToken, "elements"));
                        if(SkillsCollection != null && SkillsCollection.HasValues)
                        {
                            SkillsCollection.ForEach(skill =>
                            {
                                try
                                {
                                    var array = parser.GetJArrayElement(parser.GetJTokenValue(skill, "components", "entityComponent", "subComponents", "components"));
                                    var actionToken = array.FirstOrDefault(x => x.ToString().Contains("actionComponent"));
                                    bool.TryParse(parser.GetJTokenValue(actionToken, "components", "actionComponent", "action", "endorsedSkillAction", "endorsedSkill", "endorsedByViewer"), out bool isEndorsed);
                                    if (isEndorsed)
                                        return;
                                    var skillName = parser.GetJTokenValue(skill, "components", "entityComponent","title","text");
                                    var entityUrn = parser.GetJTokenValue(actionToken, "components", "actionComponent", "action", "endorsedSkillAction", "endorsedSkill", "entityUrn")?.Replace("urn:li:fsd_profileEndorsedSkill", "")?.Replace("(","")?.Replace(")","");
                                    var arr = entityUrn.Split(',')?.ToList();
                                    var skillID = arr?.LastOrDefault()?.ToString();
                                    if (!string.IsNullOrEmpty(skillName) && !string.IsNullOrEmpty(skillID))
                                        lstSkills.Add($"{skillName}:{skillID}");
                                }
                                catch { }
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
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
            return lstSkills;
        }

        private static void GetProfileId(string profileUrl, out string profileId, ILdFunctions objLdFunctions, out string profilePageSource)
        {
            profileUrl = $"{profileUrl}/";
            var publicIdentifier = Utils.GetBetween(profileUrl, "https://www.linkedin.com/in/", "/");
            profilePageSource = objLdFunctions.GetHtmlFromUrlForMobileRequest($"https://www.linkedin.com/voyager/api/identity/profiles/{publicIdentifier}", "");
            var Jhandler = JsonJArrayHandler.GetInstance;
            var jsonObject = JObject.Parse(profilePageSource);
            var profileI = Jhandler.GetJTokenValue(jsonObject, "entityUrn") + "/";
            profileId = Utils.GetBetween(profileI, "urn:li:fs_profile:", "/");
        }

    }
}
