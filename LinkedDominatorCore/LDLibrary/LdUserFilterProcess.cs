using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ThreadUtils;
using DominatorHouseCore;
using HtmlAgilityPack;
using LinkedDominatorCore.LDModel.Filters;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using EmbeddedBrowser;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace LinkedDominatorCore.LDLibrary
{
    public interface ILdUserFilterProcess
    {
        bool IsBrowser { get; set; }
        bool IsUserFilterActive(LDUserFilterModel objLdUserFilterModel);
        bool GetFilterStatus(string profileUrl, LDUserFilterModel ldUserFilterModel, ILdFunctions ldFunctions);
    }

    public class LdUserFilterProcess : ILdUserFilterProcess
    {
        private readonly IDelayService _delayService;
        private readonly LdDataHelper _ldDataHelper = LdDataHelper.GetInstance;

        public LdUserFilterProcess(IDelayService delayService)
        {
            _delayService = delayService;
        }

        public bool IsBrowser { get; set; }

        public bool IsUserFilterActive(LDUserFilterModel objLdUserFilterModel)
        {
            var isActive = false;
            try
            {
                isActive =
                    objLdUserFilterModel.IscheckedFilterMinimumCharacterInBio ||
                    objLdUserFilterModel.IsCheckedHasInvalidWordsCheckBox ||
                    objLdUserFilterModel.IsCheckedHasValidWordsCheckBox ||
                    objLdUserFilterModel.IsCheckedMinimumConnectionsCheckbox ||
                    objLdUserFilterModel.IsCheckedRangeofConnectionsCheckbox ||
                    objLdUserFilterModel.IsCheckedExperienceCheckbox ||
                    objLdUserFilterModel.IsCheckedEducationCheckbox || 
                    objLdUserFilterModel.IsCheckedFilterProfileImageCheckbox;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isActive;
        }

        public bool GetFilterStatus(string profileUrl, LDUserFilterModel ldUserFilterModel, ILdFunctions ldFunctions)
        {
            BrowserWindow browserWindow = null;
            string destinationUrl = string.Empty;
            BrowserAutomationExtension automation = null;
            try
            {
                #region  Member Initializations for this Method
                if (string.IsNullOrEmpty(profileUrl))
                    return true;
                var publicIdentifier = _ldDataHelper.GetPublicIdentifierFromProfileUrl(profileUrl);
                var IsSalesProfile = _ldDataHelper.IsSalesProfile(profileUrl);
                publicIdentifier = string.IsNullOrEmpty(publicIdentifier) && IsSalesProfile ? _ldDataHelper.GetPublicIdentifierFromSalesProfileUrl(ldFunctions, profileUrl) : publicIdentifier;
                if(IsBrowser)
                {
                    browserWindow = ldFunctions.BrowserWindow;
                    destinationUrl = browserWindow.CurrentUrl();
                    automation = new BrowserAutomationExtension(browserWindow);
                }
                var pageSource = ldFunctions?.GetHtmlFromUrlNormalMobileRequest(profileUrl);
                var skillsAndEndorsementUrl =
                    $"https://www.linkedin.com/voyager/api/identity/profiles/{publicIdentifier}/skillCategory?includeHiddenEndorsers=true";
                var objGetDetailedUserInfo = new GetDetailedUserInfo(_delayService);
                #endregion
                var ProfilePicUrl = string.Empty;
                var linkedInProfileId = string.Empty;
                var bio = objGetDetailedUserInfo.GetPersonalDescription(pageSource, ldFunctions, publicIdentifier, out ProfilePicUrl,out linkedInProfileId);
                if (ldUserFilterModel.IsEnableAdvanceBioAndSkills && ldFunctions != null)
                {
                    if (ldFunctions.IsBrowser)
                    {
                        // here only get skills
                        bio += SkillsAndIndustryKnowledgeFromBrowser(ldUserFilterModel, pageSource);
                        // here experience 
                        bio += UserExperienceFromBrowser(ldUserFilterModel, ldFunctions, pageSource);
                    }
                    else
                    {
                        var skillsAndEndorsementResponse =
                            ldFunctions.GetRequestUpdatedUserAgent(skillsAndEndorsementUrl);
                        bio += SkillsAndIndustryKnowledge(ldUserFilterModel, skillsAndEndorsementResponse);
                        bio += UserExperience(ldUserFilterModel, ldFunctions, pageSource, publicIdentifier);
                    }

                    bio += UserEducation(pageSource, ldFunctions, publicIdentifier, JsonJArrayHandler.GetInstance);
                }
                var connectioncount = string.Empty;
                if (ldUserFilterModel.IsCheckedRangeofConnectionsCheckbox || ldUserFilterModel.IsCheckedMinimumConnectionsCheckbox)
                {
                    connectioncount = GetConnection(ldFunctions, publicIdentifier, JsonJArrayHandler.GetInstance);
                }
                if (ldUserFilterModel.IscheckedFilterMinimumCharacterInBio ? IsFilterByBioLength(ldUserFilterModel, bio):false)
                    return false;
                if (ldUserFilterModel.IsCheckedHasInvalidWordsCheckBox? IsFilterByInvalidKeywords(ldUserFilterModel, bio):false)
                    return false;
                if (ldUserFilterModel.IsCheckedHasValidWordsCheckBox? IsFilterByValidKeywords(ldUserFilterModel, bio):false)
                    return false;
                if (ldUserFilterModel.IsCheckedMinimumConnectionsCheckbox? IsFilterByMinimumConnections(ldUserFilterModel,pageSource, connectioncount):false)
                    return false;
                if (ldUserFilterModel.IsCheckedRangeofConnectionsCheckbox? IsFilterByRangeofConnections(ldUserFilterModel, pageSource, connectioncount):false)
                    return false;
                if (ldUserFilterModel.IscheckedMinimumSkillsCount ? IsFilterByMinimumSkillsCount(ldUserFilterModel, IsBrowser ? GetUserActivityPageResponse(ref browserWindow, ref automation, LdConstants.UserActivityURL(IsSalesProfile ? linkedInProfileId: publicIdentifier, "skills")) : pageSource, ldFunctions, publicIdentifier,
                    objGetDetailedUserInfo):false)
                    return false;

                if (ldUserFilterModel.IsCheckedExperienceCheckbox? IsFilterByExperience(ldUserFilterModel,IsBrowser?GetUserActivityPageResponse(ref browserWindow, ref automation, LdConstants.UserActivityURL(IsSalesProfile ?linkedInProfileId: publicIdentifier, "experience")):pageSource, ldFunctions, publicIdentifier,
                    objGetDetailedUserInfo):false)
                    return false;

                if (ldUserFilterModel.IsCheckedEducationCheckbox? IsFilterByEducation(ldUserFilterModel,IsBrowser?GetUserActivityPageResponse(ref browserWindow, ref automation, LdConstants.UserActivityURL(IsSalesProfile ? linkedInProfileId : publicIdentifier, "education")):pageSource, ldFunctions, publicIdentifier,
                    objGetDetailedUserInfo):false)
                    return false;
                if (ldUserFilterModel.IsCheckedFilterProfileImageCheckbox ? string.IsNullOrEmpty(ProfilePicUrl): false)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                if(automation!=null && !string.IsNullOrEmpty(destinationUrl) && IsBrowser)
                    automation.LoadPageUrlAndWait(destinationUrl, 6);
            }
        }
        public string GetUserActivityPageResponse(ref BrowserWindow browserWindow,ref BrowserAutomationExtension automation, string ActivityUrl)
        {
            automation.LoadAndScroll(ActivityUrl,10,true,5000,true);
            return browserWindow.GetPageSource();
        }
        private string GetConnection(ILdFunctions ldFunctions, string publicidentifier, JsonJArrayHandler jsonJArrayHandler)
        {
            string connectioncount = string.Empty;
            try
            {
                var connectioncountresponse = ldFunctions.GetInnerLdHttpHelper().GetRequest(LdConstants.GetLDUserDetailsAPI(publicidentifier)).Response;
                var jObject = JObject.Parse(connectioncountresponse);
                connectioncount = jsonJArrayHandler.GetJTokenValue(jObject, "elements", 0, "connections", "paging", "total");
            }
            catch (Exception)
            {
            }
            return connectioncount;
        }

        public bool IsFilterByBioLength(LDUserFilterModel objLdUserFilterModel, string bio)
        {
            var isFiltered = false;
            try
            {
                if (objLdUserFilterModel.IscheckedFilterMinimumCharacterInBio
                    && (bio?.Length < objLdUserFilterModel.MinimumCharacterInBio || string.IsNullOrEmpty(bio)))
                    isFiltered = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }

        public bool IsFilterByInvalidKeywords(LDUserFilterModel objLdUserFilterModel, string bio)
        {
            var isFiltered = false;
            try
            {
                if (objLdUserFilterModel.IsCheckedHasInvalidWordsCheckBox &&
                    objLdUserFilterModel.LstInvalidWords != null &&
                    objLdUserFilterModel.LstInvalidWords.Any(x => bio.ToLower().Contains(x.ToLower())))
                    isFiltered = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }

        public bool IsFilterByValidKeywords(LDUserFilterModel objLdUserFilterModel, string bio)
        {
            var isFiltered = false;
            try
            {
                if (objLdUserFilterModel.IsCheckedHasValidWordsCheckBox
                    && objLdUserFilterModel.LstValidWords != null
                    && !objLdUserFilterModel.LstValidWords.Any(x => bio.ToLower().Contains(x.ToLower())))
                    isFiltered = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }

        public bool IsFilterByMinimumConnections(LDUserFilterModel objLdUserFilterModel, string pageSource, string connectioncount)
        {
            var isFiltered = false;
            try
            {
                var connection =string.IsNullOrEmpty(connectioncount)? Utils.GetBetween(pageSource, "\"connectionsCount\":", ","):connectioncount;
                if (string.IsNullOrEmpty(connection))
                {
                    connection = HtmlAgilityHelper.GetStringInnerTextFromClassName(pageSource,
                        "pv-top-card--list pv-top-card--list-bullet").Replace("connections", "").Replace("+", "").Trim();
                    connection = string.IsNullOrEmpty(connection) ?HtmlAgilityHelper.GetListNodesFromClassName(pageSource, "_lockup-caption_sqh8tm _bodyText_1e5nen _default_1i6ulk _sizeSmall_1e5nen _lowEmphasis_1i6ulk")?.FirstOrDefault(x=>x.InnerText.Contains("connection")||x.InnerText.Contains("connections"))?.InnerText?.Replace("connections", "")?.Replace("connection","")?.Replace("+", "")?.Trim() : connection;
                }
                if (IsBrowser)
                {
                    if (objLdUserFilterModel.IsCheckedMinimumConnectionsCheckbox
                                            && (string.IsNullOrEmpty(connection) || int.Parse(connection) < 500))
                        isFiltered = true;
                }                    
                else
                {
                    if (objLdUserFilterModel.IsCheckedMinimumConnectionsCheckbox &&
                           (string.IsNullOrEmpty(connection) || int.Parse(connection) < 500))
                        isFiltered = true;
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }


        public bool IsFilterByRangeofConnections(LDUserFilterModel objLdUserFilterModel, string pageSource, string connectioncount)
        {
            var isFiltered = false;
            try
            {
                var connection = Utils.GetBetween(pageSource, "\"connectionsCount\":", ",");
                connection = string.IsNullOrEmpty(connection) && !string.IsNullOrEmpty(connectioncount) ? connectioncount : connection;
                if (string.IsNullOrEmpty(connection))
                {
                    connection = HtmlAgilityHelper.GetStringInnerTextFromClassName(pageSource,
                        "pv-top-card--list pv-top-card--list-bullet").Replace("connections", "").Replace("+", "").Trim();
                }
                if (string.IsNullOrEmpty(connection))
                    connection = connectioncount;
                int.TryParse(connection, out var connectionCount);
                if (objLdUserFilterModel.IsCheckedRangeofConnectionsCheckbox &&
                    (string.IsNullOrEmpty(connection) || connectionCount < objLdUserFilterModel.MinimumConnections ||
                     connectionCount > objLdUserFilterModel.MaximumConnections))
                    isFiltered = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }

        public bool IsFilterByMinimumSkillsCount(LDUserFilterModel objLdUserFilterModel, string pageSource,
            ILdFunctions objLdFunctions, string publicIdentifier, GetDetailedUserInfo objGetDetailedUserInfo)
        {
            var isFiltered = false;
            try
            {
                var skills = objLdFunctions.IsBrowser
                    ? GetEndorsedSkillsNodesList(pageSource).Count.ToString()
                    : objGetDetailedUserInfo.GetSkills(true, pageSource, objLdFunctions, publicIdentifier);
                int skillsCount;

                // if skill is not parsed and if  parsed is smaller than given skip.
                if (objLdUserFilterModel.IscheckedMinimumSkillsCount && (int.TryParse(skills, out skillsCount)
                                                                         && skillsCount < objLdUserFilterModel
                                                                             .MinimumSkillsCount ||
                                                                         !int.TryParse(skills, out skillsCount)))
                    isFiltered = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }

        public bool IsFilterByExperience(LDUserFilterModel objLdUserFilterModel, string pageSource,
            ILdFunctions objLdFunctions, string publicIdentifier, GetDetailedUserInfo objGetDetailedUserInfo)
        {
            var isFiltered = false;
            try
            {
                if (!objLdUserFilterModel.IsCheckedExperienceCheckbox)
                    return isFiltered;
                var experience = "";
                if (objLdFunctions.IsBrowser)
                    experience = GetExperienceNodeList(pageSource).Count == 0 ? "" : "enough experience";
                else
                    experience =
                        objGetDetailedUserInfo.GetPastTitles(true, pageSource, objLdFunctions, publicIdentifier, "")?.Experience;

                if (string.IsNullOrEmpty(experience) || experience.Contains("elements\":[]"))
                    isFiltered = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }

        public bool IsFilterByEducation(LDUserFilterModel objLdUserFilterModel, string pageSource,
            ILdFunctions objLdFunctions, string publicIdentifier, GetDetailedUserInfo objGetDetailedUserInfo)
        {
            var isFiltered = false;
            try
            {
                int educationCount;
                var education =
                    objGetDetailedUserInfo.GetEducationCollection(true, pageSource, objLdFunctions, publicIdentifier);
                if (objLdUserFilterModel.IsCheckedEducationCheckbox &&
                    (!int.TryParse(education, out educationCount) || educationCount == 0))
                    isFiltered = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isFiltered;
        }

        public string SkillsAndIndustryKnowledgeFromBrowser(LDUserFilterModel objLdUserFilterModel,
            string skillsPageSource)
        {
            var skillsData = "";
            try
            {
                var skillsDictionaryWithValue = new Dictionary<string, string>();
                // 0,1,2...
                try
                {
                    // endorsedSkills
                    var endorsedSkills = GetEndorsedSkillsNodesList(skillsPageSource);

                    // 0,1,2...
                    foreach (var endorsedSkill in endorsedSkills)
                    {
                        var skillName = Utils.GetBetween(endorsedSkill.OuterHtml,
                            "pv-skill-category-entity__name-text t-16 t-black t-bold", "<").Replace("\">", "")?.Trim();
                        //string endorsedSkillName = JsonJArrayHandler.GetTokenElement(endorsedSkill, "originalCategoryType").ToString();
                        var endorsedSkillCount =
                            Utils.GetBetween(endorsedSkill.OuterHtml, "See", "endorsements")?.Trim();
                        if (skillsDictionaryWithValue.ContainsKey(skillName))
                            continue;
                        skillsDictionaryWithValue.Add(skillName, endorsedSkillCount);
                        skillsData += skillName + "\n";
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return skillsData;
        }

        private List<HtmlNode> GetEndorsedSkillsNodesList(string skillsPageSource)
        {
            var endorsedSkills = HtmlAgilityHelper.GetListNodesFromClassName(skillsPageSource,
                "pv-skill-category-entity__top-skill pv-skill-category-entity pb3 pt4 pv-skill-endorsedSkill-entity relative ember-view");
            endorsedSkills.AddRange(HtmlAgilityHelper.GetListNodesFromClassName(skillsPageSource,
                "pv-skill-category-entity pv-skill-category-entity--secondary pt4 pv-skill-endorsedSkill-entity relative ember-view"));
            return endorsedSkills.Count>0?endorsedSkills:HtmlAgilityHelper.GetListNodesFromAttibute(skillsPageSource,"li",AttributeIdentifierType.Id,null, "SKILLS-VIEW-DETAILS-profileTabSection-ALL-SKILLS-NONE-en-US");
        }

        public string SkillsAndIndustryKnowledge(LDUserFilterModel objLdUserFilterModel, string skillsPageSource)
        {
            var skillsData = "";
            try
            {
                var skillsDictionaryWithValue = new Dictionary<string, string>();
                var jObject = JObject.Parse(skillsPageSource);

                //object>>elements>>0,1,2...>>endorsedSkills>>0,1,2...>>skill>>name
                var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
                var elements = jsonJArrayHandler.GetTokenElement(jObject, "elements");
                // 0,1,2...
                foreach (var element in elements)
                    try
                    {
                        // endorsedSkills
                        var endorsedSkills = jsonJArrayHandler.GetTokenElement(element, "endorsedSkills");
                        // 0,1,2...
                        foreach (var endorsedSkill in endorsedSkills)
                            // skill >> name 
                            try
                            {
                                var skillName = jsonJArrayHandler.GetTokenElement(endorsedSkill, "skill", "name")
                                    .ToString();
                                //string endorsedSkillName = JsonJArrayHandler.GetTokenElement(endorsedSkill, "originalCategoryType").ToString();
                                var endorsedSkillCount = jsonJArrayHandler
                                    .GetTokenElement(endorsedSkill, "endorsementCount").ToString();
                                if (skillsDictionaryWithValue.ContainsKey(skillName))
                                    continue;
                                skillsDictionaryWithValue.Add(skillName, endorsedSkillCount);
                                skillsData += skillName + "\n";
                            }
                            catch (Exception exception)
                            {
                                exception.DebugLog();
                            }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return skillsData;
        }

        public string UserExperience(LDUserFilterModel objLdUserFilterModel, ILdFunctions objLdFunctions,
            string pageSource, string profileId = "")
        {
            var experienceAndEducation = "";
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            var requestParameters = objLdFunctions.GetInnerHttpHelper().GetRequestParameter();
            try
            {
                requestParameters.Accept = "application/vnd.linkedin.normalized+json+2.1";
                requestParameters.ContentType = null;
                objLdFunctions.GetInnerHttpHelper().SetRequestParameter(requestParameters);
                var fullExperienceAndEducation = "";
                Thread.Sleep(new Random().Next(5000, 10000));
                // hitting this api we get all experienceAndEducation details of profile which we are not getting pageSource
                var fsProfile = Utils.GetBetween(pageSource, "urn:li:fs_profile:", "\"");
                if (!string.IsNullOrEmpty(fsProfile))
                    fullExperienceAndEducation =
                        $"https://www.linkedin.com/voyager/api/identity/profiles/{fsProfile}/positionGroups?start=0&count=15";

                else
                    fullExperienceAndEducation =
                        $"https://www.linkedin.com/voyager/api/identity/profiles/{profileId}/positionGroups?start=0&count=15";

                var experiencePageSource =
                    objLdFunctions.GetInnerHttpHelper().GetRequest(fullExperienceAndEducation).Response;
                var jObject = JObject.Parse(experiencePageSource);
                var elements = jsonJArrayHandler.GetTokenElement(jObject, "included");
                experienceAndEducation += ExperienceAndEducationJsonHandling(elements);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                try
                {
                    requestParameters.Accept = null;
                    requestParameters.ContentType = "application/x-www-form-urlencoded";
                    objLdFunctions.GetInnerHttpHelper().SetRequestParameter(requestParameters);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return experienceAndEducation;
        }

        public string UserExperienceFromBrowser(LDUserFilterModel objLdUserFilterModel, ILdFunctions objLdFunctions,
            string pageSource)
        {
            var experienceAndEducation = "";
            var experienceList = GetExperienceNodeList(pageSource);
            foreach (var experience in experienceList)
            {
                var skillName = Utils.RemoveHtmlTags(experience.InnerHtml);
                experienceAndEducation += skillName + "\n";
            }

            return experienceAndEducation;
        }

        private static List<HtmlNode> GetExperienceNodeList(string pageSource)
        {
            var experienceList = HtmlAgilityHelper.GetListNodesFromClassName(pageSource,
                "artdeco-list__item pvs-list__item--line-separated pvs-list__item--one-column");
            experienceList.AddRange(HtmlAgilityHelper.GetListNodesFromClassName(pageSource,
                "pv-profile-section__sortable-card-item pv-profile-section pv-position-entity ember-view"));
            return experienceList;
        }

        private string UserEducation(string pageSource, ILdFunctions objLdFunctions, string profileId,
            JsonJArrayHandler jsonJArrayHandler)
        {
            try
            {
                var url =
                    $"https://www.linkedin.com/voyager/api/identity/profiles/{profileId}/educations?count=10&start=0";
                var educationPageSource = "{\"data\":{\"*profile\":\"urn:li:fs_profile:" +
                                          Utils.GetBetween(pageSource, "{\"data\":{\"*profile\":\"urn:li:fs_profile:",
                                              "</code>");
                if (educationPageSource == "{\"data\":{\"*profile\":\"urn:li:fs_profile:")
                    educationPageSource = objLdFunctions.GetInnerHttpHelper().GetRequest(url).Response;
                var jObject = JObject.Parse(educationPageSource);

                var elements = jsonJArrayHandler.GetTokenElement(jObject, "included");
                if (elements == null)
                    elements = jsonJArrayHandler.GetTokenElement(jObject, "elements");
                return ExperienceAndEducationJsonHandling(elements);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public string ExperienceAndEducationJsonHandling(JToken elements)
        {
            var experienceAndEducation = "";
            var jsonJArrayHandler = JsonJArrayHandler.GetInstance;
            //object>>included>>0,1,2...>>companyName or schoolName or fieldOfStudy >>
            foreach (var element in elements)
                try
                {
                    var companyName = jsonJArrayHandler.GetJTokenValue(element, "companyName");
                    var schoolName = jsonJArrayHandler.GetJTokenValue(element, "schoolName");
                    var fieldOfStudy = jsonJArrayHandler.GetJTokenValue(element, "fieldOfStudy");

                    // must contain both schoolName and fieldOfStudy or companyName
                    if (string.IsNullOrEmpty(companyName) &&
                        (string.IsNullOrEmpty(schoolName) || string.IsNullOrEmpty(fieldOfStudy)))
                        continue;

                    // post title like developer, shipping specialist.
                    var title = jsonJArrayHandler.GetJTokenValue(element, "title");
                    var description = jsonJArrayHandler.GetJTokenValue(element, "description");
                    var locationName = jsonJArrayHandler.GetJTokenValue(element, "locationName");

                    var timePeriod = jsonJArrayHandler.GetTokenElement(element, "timePeriod");

                    var startTime = jsonJArrayHandler.GetJTokenValue(timePeriod, "startDate", "year");
                    var endTime = jsonJArrayHandler.GetJTokenValue(timePeriod, "endDate", "year");

                    if (string.IsNullOrEmpty(companyName))
                        experienceAndEducation += $"{schoolName} {fieldOfStudy}";
                    else
                        experienceAndEducation += $"{companyName} {title} {description}";

                    experienceAndEducation += $"{locationName} {startTime} {endTime} \n";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return experienceAndEducation;
        }
    }
}