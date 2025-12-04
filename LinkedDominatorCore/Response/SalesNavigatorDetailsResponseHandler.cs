using System;
using System.Linq;
using System.Net;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using HtmlAgilityPack;
using LinkedDominatorCore.DetailedInfo;
using LinkedDominatorCore.LDLibrary;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UserScraperModel = LinkedDominatorCore.LDModel.SalesNavigatorScraper.UserScraperModel;
using System.Text.RegularExpressions;

namespace LinkedDominatorCore.Response
{
    public class SalesNavigatorDetailsResponseHandler : LdResponseHandler
    {
        /// <summary>
        ///     User SalesNavigatorDetailsResponseHandler
        /// </summary>
        /// <param name="salesNavigatorUserScraperModel"></param>
        /// <param name="response"></param>
        /// <param name="objLdFunctions"></param>
        /// <param name="linkedinUser"></param>
        public SalesNavigatorDetailsResponseHandler(UserScraperModel salesNavigatorUserScraperModel,
            IResponseParameter response, ILdFunctions objLdFunctions, LinkedinUser linkedinUser)
            : base(response)
        {
            try
            {
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                if (!string.IsNullOrEmpty(response.Response) && response.Response.Length < 50 &&
                    !(Success = IsValidUser = false))
                    return;
                Success = true;
                var jsonResponse = response.Response;
                int SalesUserProfilePageResponseFailedCount = 0;
                if (string.IsNullOrEmpty(jsonResponse)?true:jsonResponse.Contains("<!DOCTYPE html>"))
                {
                    var profileId = Utils.GetBetween(linkedinUser.ProfileUrl, "/lead/", ",NAME_SEARCH");
                    var authToken = LdDataHelper.GetInstance.GetAuthTokenFromSalesProfileUrl(linkedinUser.ProfileUrl);
                    var profileApi = LdConstants.GetSalesUserProfileAPI(profileId, authToken);
                    jsonResponse = objLdFunctions.GetInnerLdHttpHelper().GetRequest(profileApi).Response;
                    while (SalesUserProfilePageResponseFailedCount++ < 4 && string.IsNullOrEmpty(jsonResponse))
                        jsonResponse = objLdFunctions.GetInnerLdHttpHelper().GetRequest(profileApi).Response;
                }
                var objJObject = jArrayHandler.ParseJsonToJObject(jsonResponse);

                if (string.IsNullOrEmpty(linkedinUser.MemberId))
                    linkedinUser.MemberId = jArrayHandler.GetJTokenValue(objJObject, "objectUrn")
                        ?.Replace("urn:li:member:", "");

                if (string.IsNullOrEmpty(linkedinUser.AuthToken))
                    linkedinUser.AuthToken = jArrayHandler.GetJTokenValue(objJObject, "objectUrn")
                        ?.Replace("urn:li:member:", "");
                int.TryParse(linkedinUser.AuthToken, out int AuthToken);
                linkedinUser.AuthToken = AuthToken > 0 ? jArrayHandler.GetJTokenValue(objJObject, "entityUrn")?.Replace("(", "")?.Replace(")", "")?.Split(',')?.LastOrDefault(x => x != string.Empty) : linkedinUser.AuthToken;
                var FullName = jArrayHandler.GetJTokenValue(objJObject, "fullName");
                linkedinUser.FullName = string.IsNullOrEmpty(linkedinUser.FullName) ?string.IsNullOrEmpty(FullName)?jArrayHandler.GetJTokenValue(objJObject, "firstName") +" "+jArrayHandler.GetJTokenValue(objJObject, "lastName") :FullName:linkedinUser.FullName;
                var objSalesNavigatorUserScraperDetails = new SalesNavigatorScraperDetails(linkedinUser.MemberId)
                {
                    IsVisiting = true,
                    IsCheckedOnlyDetailsRequiredToSendConnectionRequest = salesNavigatorUserScraperModel
                        .IsCheckedOnlyDetailsRequiredToSendConnectionRequest,
                    MemberId = linkedinUser.MemberId,
                    SalesNavigatorProfileUrl = string.IsNullOrEmpty(linkedinUser.SalesNavigatorProfileUrl)? "https://www.linkedin.com/sales/lead/" + jArrayHandler.GetJTokenValue(objJObject, "entityUrn")?.Replace("urn:li:fs_salesProfile:(", "")?.Replace(")","") : linkedinUser.SalesNavigatorProfileUrl
                        
                };
                objSalesNavigatorUserScraperDetails.SalesNavigatorProfileUrl =
                    Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.SalesNavigatorProfileUrl);

                #region Filters After Visiting Profile

                #region ProfileSummary OR Bio
                var ProfileSummary = jArrayHandler.GetJTokenValue(objJObject, "summary");
                objSalesNavigatorUserScraperDetails.ProfileSummary =string.IsNullOrEmpty(ProfileSummary)?jArrayHandler.GetJTokenValue(objJObject, "headline"):ProfileSummary;
                objSalesNavigatorUserScraperDetails.ProfileSummary =
                    string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.ProfileSummary)
                        ? "N/A"
                        : Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.ProfileSummary);

                #region Bio Filters

                #region MinimumCharacterInBio Filter

                if (salesNavigatorUserScraperModel.LDUserFilterModel.IscheckedFilterMinimumCharacterInBio)
                    if (objSalesNavigatorUserScraperDetails.ProfileSummary.Length <
                        salesNavigatorUserScraperModel.LDUserFilterModel.MinimumCharacterInBio ||
                        string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.ProfileSummary))
                    {
                        IsValidUser = false;
                        return;
                    }

                #endregion


                //InvalidWords Filter
                if (salesNavigatorUserScraperModel.LDUserFilterModel.IsCheckedHasInvalidWordsCheckBox
                    && salesNavigatorUserScraperModel.LDUserFilterModel.LstInvalidWords != null
                    && salesNavigatorUserScraperModel.LDUserFilterModel.LstInvalidWords.Any(x =>
                        objSalesNavigatorUserScraperDetails.ProfileSummary.ToLower().Contains(x.ToLower())))
                {
                    IsValidUser = false;
                    return;
                }

                #endregion

                #endregion

                #region NumberOfConnections
                // NumberOfConnections
                var NumOfConnection = jArrayHandler.GetJTokenValue(objJObject, "numberOfConnections");
                NumOfConnection = string.IsNullOrEmpty(NumOfConnection) ? Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "numOfConnections")) : NumOfConnection;
                objSalesNavigatorUserScraperDetails.NumberOfConnections = NumOfConnection;
                // MinimumConnections Filter
                if (salesNavigatorUserScraperModel.LDUserFilterModel.IsCheckedMinimumConnectionsCheckbox)
                    if (string.IsNullOrEmpty(NumOfConnection) ||NumOfConnection == "N/A" ||int.Parse(NumOfConnection) < 500)
                    {
                        IsValidUser = false;
                        return;
                    }

                #endregion

                #endregion

                #region ProfilePicUrl

                try
                {
                    var ProfilePicUrl= jArrayHandler.GetJTokenValue(objJObject, "pictureId");
                    ProfilePicUrl = string.IsNullOrEmpty(ProfilePicUrl) ? jArrayHandler.GetJTokenValue(objJObject, "profilePictureDisplayImage","artifacts", 3, "fileIdentifyingUrlPathSegment") : ProfilePicUrl;
                    ProfilePicUrl = string.IsNullOrEmpty(ProfilePicUrl) ?Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "positions",0, "companyUrnResolutionResult", "companyPictureDisplayImage", "rootUrl")+jArrayHandler.GetJTokenValue(jArrayHandler.GetJArrayElement(jArrayHandler.GetJTokenValue(objJObject, "positions", 0, "companyUrnResolutionResult", "companyPictureDisplayImage", "artifacts"))?.LastOrDefault(x=>x.ToString().Contains("\"width\": 400")|| x.ToString().Contains("\"width\": 200")|| x.ToString().Contains("\"width\": 100")), "fileIdentifyingUrlPathSegment")) : ProfilePicUrl;
                    objSalesNavigatorUserScraperDetails.ProfilePicUrl =
                        Utils.InsertSpecialCharactersInCsv(ProfilePicUrl);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objSalesNavigatorUserScraperDetails.ProfilePicUrl = "N/A";
                }

                #endregion

                // profileUrl 
                objSalesNavigatorUserScraperDetails.ProfileUrl =
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "flagshipProfileUrl")); //flagshipProfileUrl
                ProfileUrl = objSalesNavigatorUserScraperDetails.ProfileUrl;

                #region Get ProfilePageSource to get Extra informations not present in API

                var salesNavigatorProfileUrl = objSalesNavigatorUserScraperDetails.SalesNavigatorProfileUrl;
                salesNavigatorProfileUrl = salesNavigatorProfileUrl.Replace("\"", "");
                ProfilePageSource = objLdFunctions.IsBrowser
                    ? response.Response
                    : objLdFunctions.GetHtmlFromUrlNormalMobileRequest(salesNavigatorProfileUrl);
                
                if (string.IsNullOrEmpty(ProfilePageSource) || ProfilePageSource == "Please Check Headers")
                    ProfilePageSource =
                        objLdFunctions.GetRequestUpdatedUserAgent(
                            salesNavigatorProfileUrl);

                #endregion

                #region FullName

                try
                {
                    objSalesNavigatorUserScraperDetails.FullName = jArrayHandler.GetJTokenValue(objJObject, "fullName");
                    if(string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.FullName))
                    {
                        objSalesNavigatorUserScraperDetails.FullName = objSalesNavigatorUserScraperDetails.ProfileUrl.Split('/').Last();
                    }
                    if(string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.FullName)&& objSalesNavigatorUserScraperDetails.ProfileUrl.Contains("-"))
                    {
                        objSalesNavigatorUserScraperDetails.FullName = objSalesNavigatorUserScraperDetails.ProfileUrl.Split('/').Last();
                        objSalesNavigatorUserScraperDetails.FullName = Regex.Replace(objSalesNavigatorUserScraperDetails.FullName.Replace("-", " "), "[^a-zA-Z]", ""); 
                    }
                    objSalesNavigatorUserScraperDetails.FullName =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.FullName);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objSalesNavigatorUserScraperDetails.FullName = "N/A";
                }

                #endregion


                if (objLdFunctions.IsBrowser)
                {
                    if (!(IsValidUser = GetSkillAndEducationForBrowserResponse(response.Response, linkedinUser,
                        objSalesNavigatorUserScraperDetails, salesNavigatorUserScraperModel)))
                        return;
                }
                else
                {
                    var profileDetailUpdatedApi =
                        $"https://www.linkedin.com/sales-api/salesApiProfiles?ids=List((profileId:{linkedinUser.ProfileId},authType:NAME_SEARCH,authToken:{linkedinUser.AuthToken}))" +
                        "&decoration=%28entityUrn%2Cinterests%2Cskills*%2Cpublications*%2Ccourses*%2Cawards*%2Clanguages*%2Corganizations*%2Cpositions*%28companyName%2Ccurrent%2Cnew%2Cdescription%2CendedOn%2CposId%2CstartedOn%2Ctitle%2Clocation%2CrichMedia*%2CcompanyUrn~fs_salesCompany%28entityUrn%2CpictureInfo%2Cname%2CcompanyPictureDisplayImage%29%29%2Ceducations*%28degree%2CeduId%2CendedOn%2CschoolName%2CstartedOn%2CfieldsOfStudy*%2CrichMedia*%2Cschool~fs_salesSchool%28entityUrn%2ClogoId%2Cname%2Curl%2CschoolPictureDisplayImage%29%29%2CvolunteeringExperiences*%28cause%2CcompanyName%2Cdescription%2CendedOn%2Crole%2CstartedOn%2Ccompany~fs_salesCompany%28entityUrn%2CpictureInfo%2Cname%2CcompanyPictureDisplayImage%29%29%2Ccertifications*%28authority%2CendedOn%2ClicenseNumber%2Cname%2CstartedOn%2Curl%2Ccompany~fs_salesCompany%28entityUrn%2CpictureInfo%2Cname%2CcompanyPictureDisplayImage%29%29%2Cpatents*%28applicationNumber%2Cdescription%2CfiledOn%2CissuedOn%2Cissuer%2Cnumber%2Ctitle%2Curl%2Cinventors*%28entityUrn~fs_salesProfile%28entityUrn%2CfirstName%2ClastName%2CfullName%2CpictureInfo%2CprofilePictureDisplayImage%29%29%29%2Cprojects*%28description%2CendedOn%2CsingleDate%2CstartedOn%2Ctitle%2Curl%2Cmembers*%28entityUrn~fs_salesProfile%28entityUrn%2CfirstName%2ClastName%2CfullName%2CpictureInfo%2CprofilePictureDisplayImage%29%29%29%29";

                    var skillsResponse = objLdFunctions.GetRequestUpdatedUserAgent(profileDetailUpdatedApi);
                    
                    if (!(IsValidUser = GetSkillAndEducation(skillsResponse, linkedinUser,
                        objSalesNavigatorUserScraperDetails, salesNavigatorUserScraperModel)))
                        return;
                }

                //DetailsRequiredToSendConnectionRequest
                var normalProfileUrl = $"https://www.linkedin.com/in/{linkedinUser.ProfileId}";
                objSalesNavigatorUserScraperDetails.DetailsRequiredToSendConnectionRequest =
                    normalProfileUrl + "<:>" + objSalesNavigatorUserScraperDetails.FullName + "<:>" +
                    linkedinUser.ProfileId;


                // ConnectionType
                objSalesNavigatorUserScraperDetails.ConnectionType =
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "degree"));

                //Location
                objSalesNavigatorUserScraperDetails.Location =
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "location"));

                //Industry
                objSalesNavigatorUserScraperDetails.Industry =
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "industry"));

                //HeadlineTitle
                objSalesNavigatorUserScraperDetails.HeadlineTitle =
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "headline"));


                //Email
                objSalesNavigatorUserScraperDetails.Email =
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "contactInfo", "emails", 0,
                        "emailAddress"));


                //PhoneNumber
                objSalesNavigatorUserScraperDetails.PhoneNumber =
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objJObject, "contactInfo", "phones", 0));


                #region CurrentCompany,CurrentTitle,PastCompanies,PastTitles and ExperienceCollection

                try
                {
                    //var allCompanies = string.Empty;
                    //var allTitles = string.Empty;
                    var experienceArray = jArrayHandler.GetJArrayElement(jArrayHandler.GetJTokenValue(objJObject, "positions"));
                    var Experience = "[";
                    foreach (var item in experienceArray)
                    {
                        // A company may have multiple works like walmart, having walmart labs, ecommerce 
                        // current may be true for all walmart works
                        var isCurrent = jArrayHandler.GetJTokenValue(item, "current");
                        if (isCurrent == "True")
                        {
                            #region CurrentCompany

                            try
                            {
                                objSalesNavigatorUserScraperDetails.CurrentCompany = jArrayHandler.GetJTokenValue(item, "companyName");
                                var currentCompanyId = jArrayHandler.GetJTokenValue(item, "companyId");
                                if (string.IsNullOrEmpty(currentCompanyId))
                                    currentCompanyId = jArrayHandler.GetJTokenValue(item, "companyUrn")
                                        ?.Replace("urn:li:fs_salesCompany:", "");
                                if (!string.IsNullOrEmpty(currentCompanyId))
                                {
                                    var actionUrl = $"https://www.linkedin.com/sales-api/salesApiCompanies/{currentCompanyId}?decoration=%28entityUrn%2Cname%2Caccount%28saved%2CnoteCount%2ClistCount%2CcrmStatus%2Cstarred%29%2CpictureInfo%2CcompanyPictureDisplayImage%2Cdescription%2Cindustry%2Clocation%2Cheadquarters%2Cwebsite%2CrevenueRange%2CcrmOpportunities%2CflagshipCompanyUrl%2CemployeeGrowthPercentages%2Cemployees*~fs_salesProfile%28entityUrn%2CfirstName%2ClastName%2CfullName%2CpictureInfo%2CprofilePictureDisplayImage%29%2Cspecialties%2Ctype%2CyearFounded%29";
                                    var currentCompanyApiResponse =objLdFunctions.IsBrowser?objLdFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response
                                        :objLdFunctions.GetHtmlFromUrlForMobileRequest(actionUrl, "");
                                    var failedCount = 0;
                                    while(failedCount++<2&&string.IsNullOrEmpty(currentCompanyApiResponse))
                                        currentCompanyApiResponse = objLdFunctions.IsBrowser ? objLdFunctions.GetInnerHttpHelper().GetRequest(actionUrl).Response
                                        : objLdFunctions.GetHtmlFromUrlForMobileRequest(actionUrl, "");
                                    GetCompanyDetails(objSalesNavigatorUserScraperDetails, currentCompanyId,
                                        currentCompanyApiResponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objSalesNavigatorUserScraperDetails.CurrentCompany = "N/A";
                            }

                            #endregion

                            // CurrentTitle
                            objSalesNavigatorUserScraperDetails.CurrentTitle =
                                Utils.AssignNa(jArrayHandler.GetJTokenValue(item, "title"));
                        }

                        if (item!=experienceArray.LastOrDefault())
                        {
                            Experience += jArrayHandler.GetJTokenValue(item, "title")?.Replace(",","")+" : ";
                            #region PastCompanies

                            try
                            {
                                if (isCurrent == "False")
                                {
                                    if (string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.PastCompanies))
                                        objSalesNavigatorUserScraperDetails.PastCompanies = jArrayHandler.GetJTokenValue(item, "companyName");
                                    else
                                        objSalesNavigatorUserScraperDetails.PastCompanies +=" <:> " + jArrayHandler.GetJTokenValue(item, "companyName");
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objSalesNavigatorUserScraperDetails.PastCompanies = "N/A";
                            }

                            #endregion

                            #region PastTitles

                            try
                            {
                                if (isCurrent == "False")
                                {
                                    if (string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.PastTitles))
                                        objSalesNavigatorUserScraperDetails.PastTitles = jArrayHandler.GetJTokenValue(item,"title");
                                    else
                                        objSalesNavigatorUserScraperDetails.PastTitles += " <:> " + jArrayHandler.GetJTokenValue(item, "title");
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objSalesNavigatorUserScraperDetails.PastTitles = "N/A";
                            }

                            #endregion
                        }
                        else
                        {
                            Experience += jArrayHandler.GetJTokenValue(item, "title");
                            #region PastCompanies

                            try
                            {
                                if (isCurrent == "False")
                                    objSalesNavigatorUserScraperDetails.PastCompanies = jArrayHandler.GetJTokenValue(item, "companyName");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objSalesNavigatorUserScraperDetails.PastCompanies = "N/A";
                            }

                            #endregion

                            #region PastTitles

                            try
                            {
                                if (isCurrent == "False")
                                    objSalesNavigatorUserScraperDetails.PastTitles = jArrayHandler.GetJTokenValue(item, "title");
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                                objSalesNavigatorUserScraperDetails.PastTitles = "N/A";
                            }

                            #endregion
                        }
                    }
                    objSalesNavigatorUserScraperDetails.Experience = Experience + "]";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objSalesNavigatorUserScraperDetails.CurrentCompany = "N/A";
                    objSalesNavigatorUserScraperDetails.CurrentTitle = "N/A";
                    objSalesNavigatorUserScraperDetails.PastCompanies = "N/A";
                    objSalesNavigatorUserScraperDetails.PastTitles = "N/A";
                    objSalesNavigatorUserScraperDetails.Experience = "N/A";
                }

                if (!string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.Experience) &&
                    objSalesNavigatorUserScraperDetails.Experience != "N/A")
                {
                    #region InsertSpecialCharactersInCsv

                    objSalesNavigatorUserScraperDetails.Experience =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.Experience);
                    objSalesNavigatorUserScraperDetails.CurrentTitle =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.CurrentTitle);
                    objSalesNavigatorUserScraperDetails.CurrentCompany =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.CurrentCompany);
                    objSalesNavigatorUserScraperDetails.PastCompanies =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.PastCompanies);
                    objSalesNavigatorUserScraperDetails.PastTitles =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.PastTitles);

                    #endregion
                }

                #endregion

                JsonObject = JsonConvert.SerializeObject(objSalesNavigatorUserScraperDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                Success = false;
                JsonObject = null;
            }
        }


        /// <summary>
        ///     CompanySalesNavigator ResponseHandler
        /// </summary>
        /// <param name="response"></param>
        /// <param name="companyUrl"></param>
        public SalesNavigatorDetailsResponseHandler(IResponseParameter response, string companyUrl) : base(response)
        {
            try
            {
                Success = !string.IsNullOrEmpty(response.Response);
                var companyId =
                    LdDataHelper.GetInstance.GetIdFromUrl(companyUrl);
                var objSalesNavigatorScraperDetails = new SalesNavigatorScraperDetails(companyId);
                GetCompanyDetails(objSalesNavigatorScraperDetails, companyId, response.Response);
                JsonObject = JsonConvert.SerializeObject(objSalesNavigatorScraperDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                Success = false;
                JsonObject = null;
            }
        }

        public string ProfilePageSource { get; }
        public string JsonObject { get; }
        public string ProfileUrl { get; }
        public bool IsValidUser { get; }

        public CurrentCompanyDetails GetCompanyDetails(CurrentCompanyDetails objCurrentCompanyDetails,
            string currentCompanyId, string currentCompanyApiResponse)
        {
            try
            {
                var jArrayHandler = JsonJArrayHandler.GetInstance;
                var jsonApiResponse = currentCompanyApiResponse;
                // here we trimming required detailed of a company from company page response
                var objAccount = GetCurrentCompanyJObject(jsonApiResponse, jArrayHandler);

                // new CurrentCompanyDetails(currentCompanyId);
                objCurrentCompanyDetails.CurrentCompanyId = currentCompanyId;

                objCurrentCompanyDetails.Name =
                    Utils.AssignNa(Utils.RemoveSpecialCharacters(jArrayHandler.GetJTokenValue(objAccount, "name")));

                // CurrentCompanyIndustryType
                objCurrentCompanyDetails.CurrentCompanyIndustryType =
                    Utils.AssignNa(Utils.RemoveSpecialCharacters(jArrayHandler.GetJTokenValue(objAccount, "industry")));
                objCurrentCompanyDetails.CurrentCompanyIndustryType =
                    Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanyIndustryType);


                // CurrentCompanyLocation 
                objCurrentCompanyDetails.CurrentCompanyLocation = Utils.RemoveSpecialCharacters(
                    Utils.AssignNa(jArrayHandler.GetJTokenValue(objAccount, "location")));
                objCurrentCompanyDetails.CurrentCompanyLocation =
                    Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanyLocation);


                // CurrentCompanySize
                if (string.IsNullOrEmpty(objCurrentCompanyDetails.CurrentCompanySize =
                    jArrayHandler.GetJTokenValue(objAccount, "fmtSize")))
                    objCurrentCompanyDetails.CurrentCompanySize = Utils.AssignNa(jArrayHandler
                        .GetJTokenValue(objAccount, "employeeCountRange")?.Replace("myself only", ""));
                objCurrentCompanyDetails.CurrentCompanySize =
                    Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanySize);

                // CurrentCompanyLogo
                if (string.IsNullOrEmpty(objCurrentCompanyDetails.CurrentCompanyLogo =
                    jArrayHandler.GetJTokenValue(objAccount, "pictureId")))
                    objCurrentCompanyDetails.CurrentCompanyLogo = Utils.AssignNa(
                        jArrayHandler.GetJTokenValue(objAccount, "companyPictureDisplayImage", "rootUrl")+ jArrayHandler.GetJTokenValue(jArrayHandler.GetJArrayElement(jArrayHandler.GetTokenElement(objAccount, "companyPictureDisplayImage", "artifacts").ToString()).LastOrDefault(x => x.ToString().Contains("\"width\": 400") || x.ToString().Contains("\"width\": 200") || x.ToString().Contains("\"width\": 100")), "fileIdentifyingUrlPathSegment"));
                objCurrentCompanyDetails.CurrentCompanyLogo =
                    Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanyLogo);
                //CurrentCompanyDescription
                objCurrentCompanyDetails.CurrentCompanyDescription = Utils.RemoveSpecialCharacters(
                    jArrayHandler.GetJTokenValue(objAccount, "description"));
                objCurrentCompanyDetails.CurrentCompanyDescription =
                    Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanyDescription);


                // CurrentCompanyFoundingYear
                objCurrentCompanyDetails.CurrentCompanyFoundingYear =
                    jArrayHandler.GetJTokenValue(objAccount, "yearFounded");
                if (string.IsNullOrEmpty(objCurrentCompanyDetails.CurrentCompanyFoundingYear))
                    objCurrentCompanyDetails.CurrentCompanyFoundingYear =
                        Utils.GetBetween(objCurrentCompanyDetails.CurrentCompanyDescription, "founded in", " ");
                objCurrentCompanyDetails.CurrentCompanyFoundingYear =
                    Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanyFoundingYear);

                if (string.IsNullOrEmpty(objCurrentCompanyDetails.CurrentCompanyFoundingYear))
                    objCurrentCompanyDetails.CurrentCompanyFoundingYear = "N/A";

                //CurrentCompanyHeadquarters
                var HeadQuarterNode = jArrayHandler.GetJTokenOfJToken(objAccount, "headquarters");
                objCurrentCompanyDetails.CurrentCompanyHeadquarters =
                    Utils.AssignNa(Utils.RemoveSpecialCharacters(jArrayHandler.GetJTokenValue(HeadQuarterNode, "line1")+" "+jArrayHandler.GetJTokenValue(HeadQuarterNode,"city")+" "+jArrayHandler.GetJTokenValue(HeadQuarterNode, "geographicArea")+" "+jArrayHandler.GetJTokenValue(HeadQuarterNode, "postalCode")+" "+jArrayHandler.GetJTokenValue(HeadQuarterNode, "country")));
                objCurrentCompanyDetails.CurrentCompanyHeadquarters =
                    Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanyHeadquarters);

                #region CurrentCompanyWebsite

                try
                {
                    objCurrentCompanyDetails.CurrentCompanyWebsite = objAccount["website"].ToString();
                    objCurrentCompanyDetails.CurrentCompanyWebsite =
                        Utils.InsertSpecialCharactersInCsv(objCurrentCompanyDetails.CurrentCompanyWebsite);
                }
                catch (Exception)
                {
                    //
                    objCurrentCompanyDetails.CurrentCompanyWebsite = "N/A";
                }

                #endregion

                return objCurrentCompanyDetails;
            }
            catch (Exception)
            {
                //
                return new CurrentCompanyDetails();
            }
        }


        public JObject GetCurrentCompanyJObject(string currentCompanyApiResponse, JsonJArrayHandler jArrayHandler)
        {
            JObject jObject = null;
            try
            {
                if (!currentCompanyApiResponse.Contains("<!DOCTYPE html>"))
                    return jArrayHandler.ParseJsonToJObject(currentCompanyApiResponse);

                var decodedResponse = WebUtility.HtmlDecode(currentCompanyApiResponse);
                var jsonApiResponse =
                    "{\"website\":\"" + Utils.GetBetween(decodedResponse, "{\"website\":\"", "<").Trim();
                jObject = jArrayHandler.ParseJsonToJObject(jsonApiResponse);
                if (jObject != null)
                    return jObject;
                jsonApiResponse = "{\"description\":\"" +
                                  Utils.GetBetween(decodedResponse, "{\"description\":\"", "<").Trim();
                jObject = jArrayHandler.ParseJsonToJObject(jsonApiResponse);

                return jObject;
            }
            catch (Exception)
            {
                return jObject;
            }
        }

        public bool GetSkillAndEducation(string skillsResponse, LinkedinUser linkedinUser,
            SalesNavigatorScraperDetails objSalesNavigatorUserScraperDetails,
            UserScraperModel salesNavigatorUserScraperModel)
        {
            var jArrayHandler = JsonJArrayHandler.GetInstance;

            try
            {
                var skillsAndEducationObject = jArrayHandler.ParseJsonToJObject(skillsResponse);
                var results = jArrayHandler.GetJTokenOfJToken(skillsAndEducationObject,"results");
                var accountSkillsAndEducationResults =jArrayHandler.GetJTokenOfJToken(results,$"(authToken:{linkedinUser.AuthToken},authType:NAME_SEARCH,profileId:{linkedinUser.ProfileId})");

                #region Skills

                JArray skillsArray = null;
                var Skills = "[";
                try
                {
                    skillsArray = jArrayHandler.GetJArrayElement(jArrayHandler.GetJTokenValue(accountSkillsAndEducationResults,"skills"));
                    foreach (var item in skillsArray)
                    {
                        if(item!=skillsArray.LastOrDefault())
                            Skills += jArrayHandler.GetJTokenValue(item, "name")+" : ";
                        else
                            Skills += jArrayHandler.GetJTokenValue(item, "name");
                    }
                    objSalesNavigatorUserScraperDetails.Skills = Skills + "]";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objSalesNavigatorUserScraperDetails.Skills = "N/A";
                }

                if (!string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.Skills) &&
                    objSalesNavigatorUserScraperDetails.Skills != "N/A")
                    objSalesNavigatorUserScraperDetails.Skills =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.Skills);

                #region MinimumSkills Filter

                if (skillsArray != null &&
                    salesNavigatorUserScraperModel.LDUserFilterModel.IscheckedMinimumSkillsCount &&
                    skillsArray.Count < salesNavigatorUserScraperModel.LDUserFilterModel.MinimumSkillsCount)
                    return false;

                #endregion

                #endregion

                #region Education

                try
                {
                    var education = jArrayHandler.GetJTokenValue(accountSkillsAndEducationResults, "educations");
                    var educationArray =jArrayHandler.GetJArrayElement(education);
                    var Education = "[";
                    foreach (var item in educationArray)
                    {
                        var degree = jArrayHandler.GetJTokenValue(item, "degree");
                        var schoolName = jArrayHandler.GetJTokenValue(item, "schoolName");
                        var details = string.IsNullOrEmpty(degree) ? string.IsNullOrEmpty(schoolName) ? string.Empty : schoolName : string.IsNullOrEmpty(schoolName) ? degree : degree + " : "+schoolName;
                        if (item != educationArray.LastOrDefault())
                            Education += details + " :: ";
                        else
                            Education += details;
                    }
                    objSalesNavigatorUserScraperDetails.Education = Education + "]";
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objSalesNavigatorUserScraperDetails.Education = "N/A";
                }

                if (!string.IsNullOrEmpty(objSalesNavigatorUserScraperDetails.Education) &&
                    objSalesNavigatorUserScraperDetails.Education != "N/A")
                {
                    objSalesNavigatorUserScraperDetails.Education =
                        Utils.InsertSpecialCharactersInCsv(objSalesNavigatorUserScraperDetails.Education);
                }
                else
                {
                    #region Education Filter

                    if (salesNavigatorUserScraperModel.LDUserFilterModel.IsCheckedEducationCheckbox) return false;

                    #endregion
                }

                #endregion
            }
            catch (Exception)
            {
                //
            }

            return true;
        }

        public bool GetSkillAndEducationForBrowserResponse(string skillsResponse, LinkedinUser linkedinUser,
            SalesNavigatorScraperDetails salesScraperDetails,
            UserScraperModel scraperModel)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(WebUtility.HtmlDecode(Response.Response));
            try
            {
                #region Skills

                var skillsArray =
                    HtmlAgilityHelper.GetListNodesFromClassName("", "skills-item _gridListItem_levz8k _listItem_f6tp8x", htmlDoc);
                skillsArray = skillsArray?.Count > 0 ? skillsArray : HtmlAgilityHelper.GetListNodesFromClassName(string.Empty, "_skills-item_1fae2i _gridListItem_levz8k _listItem_f6tp8x", htmlDoc);
                var Skills = "[";
                foreach (var skill in skillsArray)
                {
                    var item = HtmlAgilityHelper.GetStringInnerTextFromClassName(skill.InnerHtml, "skill-name nowrap-ellipsis _bodyText_1e5nen _default_1i6ulk _weightBold_1e5nen");
                    if (skill != skillsArray.LastOrDefault())
                        Skills += item + " : ";
                    else
                        Skills += item;
                }
                Skills += "]";
                salesScraperDetails.Skills = Skills=="[]"
                    ? "N/A"
                    : Utils.InsertSpecialCharactersInCsv(Skills);

                if (skillsArray != null && scraperModel.LDUserFilterModel.IscheckedMinimumSkillsCount &&
                    skillsArray.Count < scraperModel.LDUserFilterModel.MinimumSkillsCount) return false;

                #endregion

                #region Education

                var educationArray = HtmlAgilityHelper.GetListNodesFromClassName("","profile-education__school-name Sans-16px-black-90%-bold-open", htmlDoc);
                educationArray = educationArray?.Count > 0 ? educationArray : HtmlAgilityHelper.GetListNodesFromClassName(string.Empty, "education-entry", htmlDoc);
                var Education = "[";
                foreach (var item in educationArray)
                {
                    var Degrees = HtmlAgilityHelper.GetListInnerTextFromTagName(Regex.Replace(HtmlAgilityHelper.GetStringInnerHtmlFromClassName(item.InnerHtml, "_bodyText_1e5nen _default_1i6ulk _sizeSmall_1e5nen _default_1i6ulk _weightDefault_1e5nen")?.Trim()?.Replace("\r","")?.Replace("\n","")?.Replace("\t",""),@"\s+",""), HTMLTags.Span);
                    var degree = string.Join(" ", Degrees);
                    var schoolName = HtmlAgilityHelper.GetStringInnerTextFromClassName(item, "ember-view _school-link_aos46q");
                    schoolName = string.IsNullOrEmpty(schoolName) ?HtmlAgilityHelper.GetStringInnerTextFromClassName(item.InnerHtml, "_bodyText_1e5nen _default_1i6ulk _sizeMedium_1e5nen _weightBold_1e5nen") : schoolName;
                    var details = string.IsNullOrEmpty(degree) ? string.IsNullOrEmpty(schoolName) ? string.Empty : schoolName : string.IsNullOrEmpty(schoolName) ? degree : degree + " : " + schoolName;
                    if (item != educationArray.LastOrDefault())
                        Education += details + " :: ";
                    else
                        Education += details;
                }
                salesScraperDetails.Education = Education += "]";
                if (string.IsNullOrEmpty(salesScraperDetails.Education) &&
                    scraperModel.LDUserFilterModel.IsCheckedEducationCheckbox)
                    return false;

                salesScraperDetails.Education = string.IsNullOrEmpty(salesScraperDetails.Education)
                    ? "N/A"
                    : Utils.InsertSpecialCharactersInCsv(salesScraperDetails.Education);

                #endregion
            }
            catch (Exception)
            {
                //
            }

            return true;
        }
    }
}