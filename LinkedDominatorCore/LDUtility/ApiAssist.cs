using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DominatorHouseCore;

namespace LinkedDominatorCore.LDUtility
{
    public class ApiAssist
    {
        private readonly LdDataHelper _ldDataHelper = LdDataHelper.GetInstance;
        private readonly Dictionary<Predicate<string>, Action<ApiConstructor>> _salesApiActionDictionary;

        private string _salesDesktopUrl;

        public ApiAssist()
        {
            try
            {
                //  check is contains current filter and equals to calling respective method
                _salesApiActionDictionary = new Dictionary<Predicate<string>, Action<ApiConstructor>>
                {
                    {filter => IsExecutable(filter, FilterConstants.CompanySize), GetCompanySize},
                    {filter => IsExecutable(filter, FilterConstants.CompanyType), GetCompanyType},
                    {filter => IsExecutable(filter, FilterConstants.CompanyHeadcount), GetCompanyHeadcount},
                    {filter => IsExecutable(filter, FilterConstants.DepartmentSize), GetDepartmentSize},
                    {filter => IsExecutable(filter, FilterConstants.DepartmentGrowth), GetDepartmentGrowth},
                    {filter => IsExecutable(filter, FilterConstants.JobOpportunities), GetJobOpportunities},
                    {filter => IsExecutable(filter, FilterConstants.Fortune), GetFortune},
                    {filter => IsExecutable(filter, FilterConstants.Functions), GetFunctions},
                    {filter => IsExecutable(filter, FilterConstants.FunctionsIncluded), GetFunctionPartOfSalesUserApi},
                    {filter => IsExecutable(filter, FilterConstants.Keywords), GetKeywords},
                    {filter => IsExecutable(filter, FilterConstants.NumOfFollowers), GetNumOfFollowers},
                    {filter => IsExecutable(filter, FilterConstants.Relationship), GetRelationship},
                    {filter => IsExecutable(filter, FilterConstants.TechnologiesUsed), GetTechnologiesUsed},
                    {filter => IsExecutable(filter, FilterConstants.SearchSessionId), GetSearchSessionId},
                    {filter => IsExecutable(filter, FilterConstants.SeniorityLevel), GetSeniorityLevel},
                    {filter => IsExecutable(filter, FilterConstants.LogId), GetLogId},
                    {filter => !_salesDesktopUrl.Contains(filter), DemoType}
                };
            }
            catch (Exception exception)
            {
                exception.DebugLog();
            }
        }

        public bool IsEncodeCompanyName { get; set; } = false;

        private bool IsExecutable(string filter, string filterConstants)
        {
            return _salesDesktopUrl.Contains(filter) && filter.Equals(filterConstants);
        }


        public Predicate<string> InvokeData(string filterType)
        {
            return _salesApiActionDictionary.Keys.First(x => x.Invoke(filterType));
        }

        public string GetNewSalesNavApiCompanyType(string desktopUrl)
        {
            if (string.IsNullOrEmpty(desktopUrl))
                return "";

            var tempDesktopUrl = HttpUtility.UrlDecode(desktopUrl);
            _salesDesktopUrl = tempDesktopUrl;
            var apiConstructor = new ApiConstructor
            {
                Api =
                    "https://www.linkedin.com/sales-api/salesApiCompanySearch?q=companySearchQuery&start=0&count=40&query=("
            };
            try
            {
                _salesApiActionDictionary[InvokeData(FilterConstants.CompanyHeadcount)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.CompanySize)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.CompanyType)].Invoke(apiConstructor);

                _salesApiActionDictionary[InvokeData(FilterConstants.DepartmentSize)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.DepartmentGrowth)].Invoke(apiConstructor);

                if (Utils.IsStringContainsAnyFromParams(tempDesktopUrl, FilterConstants.Geography, FilterConstants.Geo))
                    GetGeoPartOfSalesUserApi(tempDesktopUrl, apiConstructor);

                if (Utils.IsStringContainsAnyFromParams(tempDesktopUrl, "industryIncluded=", "industry="))
                    GetIndustryPartOfSalesUserApi(apiConstructor);

                _salesApiActionDictionary[InvokeData(FilterConstants.JobOpportunities)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.Fortune)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.Keywords)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.Relationship)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.NumOfFollowers)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.Functions)].Invoke(apiConstructor);
                apiConstructor.Api += "spotlightParam:(selectedType:ALL),";
                _salesApiActionDictionary[InvokeData(FilterConstants.TechnologiesUsed)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.SeniorityLevel)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.LogId)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.SearchSessionId)].Invoke(apiConstructor);

                apiConstructor.Api = apiConstructor.Api.Trim(',');
                apiConstructor.Api = apiConstructor.Api + apiConstructor.CompanyDecoration;

                return apiConstructor.Api;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public string GetDeleteConversationApiUrl(long currentTimeInMillis)
        {
            return
                $"https://www.linkedin.com/voyager/api/messaging/conversations?keyVersion=LEGACY_INBOX&createdBefore={currentTimeInMillis}";
        }

        public string CompanyPostsApiConstantPagination(string companyId, string paginationToken, int start)
        {
            var api =
                $"https://www.linkedin.com/voyager/api/organization/updatesV2?companyIdOrUniversalName={companyId}&count=40&moduleKey=ORGANIZATION_MEMBER_FEED_DESKTOP&numComments=0&numLikes=0&q=companyRelevanceFeed&start={start}";
            api += start == 0 ? "" : $"&paginationToken={paginationToken}";
            return api;
        }

        #region sales user api construction

        public string GetNewSalesNavApiPeoplesType(string desktopUrl)
        {
            try
            {
                //',doFetchFilters:true' from this part remaining api part is same therefore only check before it
                // write api in order so that easy to check 
                if (string.IsNullOrEmpty(desktopUrl))
                    return "";
                var tempDesktopUrl = HttpUtility.UrlDecode(desktopUrl);
                _salesDesktopUrl = tempDesktopUrl;
                var apiConstructor = new ApiConstructor
                {
                    Api =
                        "https://www.linkedin.com/sales-api/salesApiPeopleSearch?q=peopleSearchQuery&start=0&count=40&query=("
                };

                #region API Construction For Peoples Type

                _salesApiActionDictionary[InvokeData(FilterConstants.CompanySize)].Invoke(apiConstructor);
                _salesApiActionDictionary[InvokeData(FilterConstants.CompanyType)].Invoke(apiConstructor);


                if (Utils.IsStringContainsAnyFromParams(tempDesktopUrl, "companyIncluded=", "company="))
                    GetCompanyPartOfSalesUserApi(tempDesktopUrl, apiConstructor);

                _salesApiActionDictionary[InvokeData(FilterConstants.FunctionsIncluded)].Invoke(apiConstructor);


                if (Utils.IsStringContainsAnyFromParams(tempDesktopUrl, "geoIncluded=", "geo="))
                    GetGeoPartOfSalesUserApi(tempDesktopUrl, apiConstructor);

                if (Utils.IsStringContainsAnyFromParams(tempDesktopUrl, "industryIncluded=", "industry="))
                    GetIndustryPartOfSalesUserApi(apiConstructor);


                if (tempDesktopUrl.Contains("excludeViewedLeads="))
                {
                    apiConstructor.ExcludeViewed = _ldDataHelper.GetApiData(tempDesktopUrl, "excludeViewedLeads=")
                        .Replace("%2C", ",");
                    apiConstructor.Api = apiConstructor.Api + "excludeViewed:" + apiConstructor.ExcludeViewed + ",";
                }

                _salesApiActionDictionary[InvokeData(FilterConstants.Keywords)].Invoke(apiConstructor);

                _salesApiActionDictionary[InvokeData(FilterConstants.Relationship)].Invoke(apiConstructor);

                _salesApiActionDictionary[InvokeData(FilterConstants.LogId)].Invoke(apiConstructor);

                if (Utils.IsStringContainsAnyFromParams(tempDesktopUrl, "seniorityIncluded=", "seniority="))
                    GetSeniorityPartOfSalesUserApi(apiConstructor);

                if (tempDesktopUrl.Contains("yearsOfExperience="))
                {
                    apiConstructor.Experience = _ldDataHelper.GetApiData(tempDesktopUrl, "yearsOfExperience=");
                    apiConstructor.Api = $"{apiConstructor.Api}yearsOfExperience:List({apiConstructor.Experience}),";
                }

                if (tempDesktopUrl.Contains("searchSessionId="))
                    GetSessionPartOfSalesUserApi(apiConstructor, tempDesktopUrl);

                apiConstructor.Api = apiConstructor.Api.Trim(',');
                apiConstructor.Api = apiConstructor.Api + apiConstructor.UserDecoration;

                #endregion

                return apiConstructor.Api;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        // only used to execute when no dictionary predicate matched
        private void DemoType(ApiConstructor apiConstructor)
        {
        }

        #endregion

        #region filterMethods

        private void GetCompanyType(ApiConstructor apiConstructor)
        {
            apiConstructor.CompanyType = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.CompanyType);
            apiConstructor.Api = apiConstructor.Api + "companyType:List(" + apiConstructor.CompanyType + "),";
        }

        private void GetSearchSessionId(ApiConstructor apiConstructor)
        {
            apiConstructor.SearchSessionId =
                _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.SearchSessionId);
            apiConstructor.Api = $"{apiConstructor.Api}trackingParam:(sessionId:{apiConstructor.SearchSessionId}),";
        }

        private void GetLogId(ApiConstructor apiConstructor)
        {
            apiConstructor.LogId = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.LogId);
            apiConstructor.Api =
                $"{apiConstructor.Api}searchHistoryParam:(doLogHistory:true,id:{apiConstructor.LogId}),";
        }

        private void GetSeniorityLevel(ApiConstructor apiConstructor)
        {
            apiConstructor.SeniorityLevel = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.SeniorityLevel);
            apiConstructor.Api = apiConstructor.Api + "seniority:List(" + apiConstructor.SeniorityLevel + "),";
        }

        private void GetTechnologiesUsed(ApiConstructor apiConstructor)
        {
            apiConstructor.TechnologiesUsed =
                _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.TechnologiesUsed);
            if (!string.IsNullOrEmpty(apiConstructor.TechnologiesUsed))
                apiConstructor.TechnologiesUsed = apiConstructor.TechnologiesUsed.Replace(":", "%3A")
                    .Replace(" ", "%20")
                    .Replace("/", "%2F");
            apiConstructor.Api = $"{apiConstructor.Api}technologiesUsed:List({apiConstructor.TechnologiesUsed}),";
        }

        private void GetFunctions(ApiConstructor apiConstructor)
        {
            apiConstructor.Functions = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.Functions);
            apiConstructor.Api = apiConstructor.Api + "function:List(" + apiConstructor.Functions + "),";
        }

        private void GetNumOfFollowers(ApiConstructor apiConstructor)
        {
            apiConstructor.NumOfFollowers = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.NumOfFollowers);
            apiConstructor.Api = $"{apiConstructor.Api}numOfFollowers:List({apiConstructor.NumOfFollowers}),";
        }

        private void GetRelationship(ApiConstructor apiConstructor)
        {
            apiConstructor.Relationship = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.Relationship);
            apiConstructor.Api = $"{apiConstructor.Api}relationship:List({apiConstructor.Relationship}),";
        }

        private void GetKeywords(ApiConstructor apiConstructor)
        {
            if (!string.IsNullOrEmpty(apiConstructor.Keywords =
                _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.Keywords)))
                apiConstructor.Keywords = apiConstructor.Keywords.Replace(":", "%3A").Replace(" ", "%20");
            apiConstructor.Api = $"{apiConstructor.Api}keywords:{apiConstructor.Keywords},";
        }

        private void GetFortune(ApiConstructor apiConstructor)
        {
            apiConstructor.Fortune = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.Fortune);
            apiConstructor.Api = apiConstructor.Api + "fortune:List(" + apiConstructor.Fortune + "),";
        }

        private void GetJobOpportunities(ApiConstructor apiConstructor)
        {
            apiConstructor.JobOpportunities =
                _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.JobOpportunities);
            apiConstructor.Api = $"{apiConstructor.Api}jobOpportunity:List({apiConstructor.JobOpportunities}),";
        }

        private void GetDepartmentSize(ApiConstructor apiConstructor)
        {
            apiConstructor.DepartmentSize = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.DepartmentSize);
            apiConstructor.Api = apiConstructor.Api + "departmentSize:" + apiConstructor.DepartmentSize + ",";
        }


        private void GetCompanyHeadcount(ApiConstructor apiConstructor) //companyGrowth
        {
            apiConstructor.CompanyHeadcount =
                _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.CompanyHeadcount);
            apiConstructor.Api = $"{apiConstructor.Api}companyGrowth:{apiConstructor.CompanyHeadcount},";
        }

        private void GetDepartmentGrowth(ApiConstructor apiConstructor)
        {
            apiConstructor.DepartmentGrowth =
                _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.DepartmentGrowth);
            apiConstructor.Api = $"{apiConstructor.Api}departmentGrowth:{apiConstructor.DepartmentGrowth},";
        }

        private void GetCompanySize(ApiConstructor apiConstructor)
        {
            apiConstructor.CompanySize = _ldDataHelper.GetApiData(_salesDesktopUrl, FilterConstants.CompanySize);
            apiConstructor.Api = $"{apiConstructor.Api}companySize:List({apiConstructor.CompanySize}),";
        }

        public void GetCompanyPartOfSalesUserApi(string desktopUrl, ApiConstructor apiConstructor)
        {
            var trimmedCompanies = _ldDataHelper.GetApiData(desktopUrl, FilterConstants.CompanyIncluded);
            if (string.IsNullOrEmpty(trimmedCompanies))
                trimmedCompanies = _ldDataHelper.GetApiData(desktopUrl, "company=");

            foreach (var url in trimmedCompanies.Split(',').ToList())
            {
                var companyNameValue = url.Split(':');
                // UrlPathEncode replaces space with %20 while UrlEncode not
                var tempPathEncode = HttpUtility.UrlPathEncode(companyNameValue[0]);
                // UrlEncode uses here because sometimes we have to encode url again
                var encodedCompanyName = IsEncodeCompanyName
                    ? HttpUtility.UrlEncode(tempPathEncode)
                    : tempPathEncode;
                apiConstructor.Company += $"(text:{encodedCompanyName},id:{companyNameValue[1]}),";
            }

            apiConstructor.Company = apiConstructor.Company.Trim(',');
            apiConstructor.Api = apiConstructor.Api +
                                 $"companyV2:(scope:CURRENT,includedValues:List({apiConstructor.Company})),";
        }

        public void GetIndustryPartOfSalesUserApi(ApiConstructor apiConstructor)
        {
            string trimmedIndustry;
            if (string.IsNullOrEmpty(trimmedIndustry = _ldDataHelper.GetApiData(_salesDesktopUrl, "industryIncluded=")))
                trimmedIndustry = _ldDataHelper.GetApiData(_salesDesktopUrl, "industry=");

            foreach (var url in trimmedIndustry.Split(',').ToList())
                apiConstructor.Industry += $"(id:{url}),";
            apiConstructor.Industry = apiConstructor.Industry.Trim(',');
            apiConstructor.Api = apiConstructor.Api + "industryV2:(includedValues:List(" + apiConstructor.Industry +
                                 ")),";
        }

        public void GetFunctionPartOfSalesUserApi(ApiConstructor apiConstructor)
        {
            apiConstructor.Functions =
                _ldDataHelper.GetApiData(_salesDesktopUrl, "functionIncluded=").Replace("%2C", ",");

            var tempData = "";
            if (apiConstructor.Functions.Contains(','))
                tempData = apiConstructor.Functions.Split(',')
                    .Aggregate(tempData, (current, data) => current + $"(id:{data}),");
            else
                tempData += $"(id:{apiConstructor.Functions}),";

            tempData = tempData.Trim(',');
            apiConstructor.Api = $"{apiConstructor.Api}functionV2:(includedValues:List({tempData})),";
        }

        public void GetSeniorityPartOfSalesUserApi(ApiConstructor apiConstructor)
        {
            if (string.IsNullOrEmpty(apiConstructor.SeniorityLevel =
                _ldDataHelper.GetApiData(_salesDesktopUrl, "seniorityIncluded=")))
                apiConstructor.SeniorityLevel = _ldDataHelper.GetApiData(_salesDesktopUrl, "seniority=");
            var tempData = "";
            if (apiConstructor.SeniorityLevel.Contains(','))
                tempData = apiConstructor.SeniorityLevel.Split(',')
                    .Aggregate(tempData, (current, data) => current + $"(id:{data}),");
            else
                tempData += $"(id:{apiConstructor.SeniorityLevel}),";

            tempData = tempData.Trim(',');
            apiConstructor.Api = $"{apiConstructor.Api}seniorityLevelV2:(includedValues:List({tempData})),";
        }

        public void GetSessionPartOfSalesUserApi(ApiConstructor apiConstructor, string tempDesktopUrl)
        {
            apiConstructor.SearchSessionId = _ldDataHelper.GetApiData(tempDesktopUrl, "searchSessionId=");
            apiConstructor.Api = apiConstructor.Api + "spotlightParam:(selectedType:ALL),trackingParam:(sessionId:" +
                                 apiConstructor.SearchSessionId + ")";
        }

        public void GetGeoPartOfSalesUserApi(string tempDesktopUrl, ApiConstructor apiConstructor)
        {
            var trimmedGeoLocation = _ldDataHelper.GetApiData(tempDesktopUrl, FilterConstants.Geography);
            if (string.IsNullOrEmpty(trimmedGeoLocation))
                trimmedGeoLocation = _ldDataHelper.GetApiData(tempDesktopUrl, FilterConstants.Geo);

            foreach (var url in trimmedGeoLocation.Split(',').ToList())
                apiConstructor.Geography += $"(id:{url.Replace(":", "%3A")}),";
            apiConstructor.Api =
                $"{apiConstructor.Api}geoV2:(includedValues:List({apiConstructor.Geography.Trim(',')})),";
        }

        #endregion
    }

    public class FilterConstants
    {
        public const string ExcludeViewed = "excludeViewedLeads=";
        public const string Keywords = "keywords=";
        public const string CompanySize = "companySize=";
        public const string CompanyType = "companyType=";
        public const string Company = "company=";
        public const string CompanyIncluded = "companyIncluded=";
        public const string CompanyHeadcount = "companyHeadCountGrowth=";
        public const string DepartmentSize = "departmentSize=";
        public const string DepartmentGrowth = "departmentHeadcountGrowth=";
        public const string Functions = "function=";
        public const string FunctionsIncluded = "functionIncluded=";
        public const string Fortune = "fortune=";
        public const string NumOfFollowers = "numOfFollowers=";
        public const string Geo = "geo=";
        public const string Geography = "geoIncluded=";
        public const string Industry = "industryIncluded=";
        public const string IndustryIncluded = "industryIncluded=";
        public const string JobOpportunities = "jobOpportunities=";
        public const string LogId = "logId=";
        public const string Relationship = "relationship=";
        public const string SeniorityLevel = "seniority=";
        public const string SearchSessionId = "searchSessionId=";

        public const string TechnologiesUsed = "technologiesUsed=";
    }

    public class ApiConstructor
    {
        public string Title { get; set; }
        public string SeniorityLevel { get; set; }
        public string Tag { get; set; }
        public string LogId { get; set; }
        public string SearchSessionId { get; set; }
        public string Experience { get; set; }

        public string UserDecoration { get; set; } =
            ",doFetchFilters:true,doFetchHits:true,doFetchSpotlights:true)&decoration=%28degree%2CentityUrn%2CfirstName%2ClastName%2CfullName%2CobjectUrn%2CgeoRegion%2Chighlight%28com.linkedin.sales.profile.profileHighlights.MentionedInTheNewsHighlight!_nt%3Dcom.linkedin.sales.deco.common.profile.highlights.DecoratedMentionedInTheNewsHighlight%28articleName%2Ccount%2Csource%2Curl%29%2Ccom.linkedin.sales.profile.profileHighlights.ProfileHighlights!_nt%3Dcom.linkedin.sales.deco.common.profile.highlights.DecoratedProfileHighlights%28sharedConnection%28sharedConnectionUrns*~fs_salesProfile%28entityUrn%2CfirstName%2ClastName%2CfullName%2CpictureInfo%2CprofilePictureDisplayImage%29%29%2CteamlinkInfo%28totalCount%29%2CsharedEducations*%28overlapInfo%2CentityUrn~fs_salesSchool%28entityUrn%2ClogoId%2Cname%2Curl%2CschoolPictureDisplayImage%29%29%2CsharedExperiences*%28overlapInfo%2CentityUrn~fs_salesCompany%28companyPictureDisplayImage%2CentityUrn%2Cname%2CpictureInfo%29%29%2CsharedGroups*%28entityUrn~fs_salesGroup%28entityUrn%2Cname%2ClargeLogoId%2CsmallLogoId%2CgroupPictureDisplayImage%29%29%29%2Ccom.linkedin.sales.profile.profileHighlights.RecentPositionChangeHighlight!_nt%3Dcom.linkedin.sales.deco.common.profile.highlights.DecoratedRecentPositionChangeHighlight%28companyUrn%2CcompanyName%2Cduration%2Ctitle%29%29%2CsharedConnectionsHighlight%2CteamlinkIntrosHighlight%2CopenLink%2Cpremium%2Cteamlink%2CpendingInvitation%2CprofilePictureDisplayImage%2Cviewed%2Csaved%2CcrmStatus%2CtrackingId%2CrecommendedLeadTrackingId%2ClistCount%2CcurrentPositions*%2Ctags*%2CpastPositions*%2CmatchedArticles*%2CfacePiles*%29";

        public string CompanyDecoration { get; set; } =
            ",doFetchFilters:true,doFetchHits:true,doFetchSpotlights:true)&decoration=%28companyName%2CcompanyPictureDisplayImage%2CcrmStatus%2Cdescription%2CemployeeCountRange%2CentityUrn%2CfirstConnectionsHighlight%2Cindustry%2ClistCount%2Clocation%2CpictureInfo%2CrevenueHighlights%2Csaved%2CsavedLeadCount%2CseniorHiresHighlight%2CtrackingId%2Ctags*%2CfacePiles*%2CtechnologiesUsedHighlights*%29";

        #region MyRegion

        public string Api { get; set; } = string.Empty;
        public string ExcludeViewed { get; set; }
        public string Keywords { get; set; }
        public string CompanySize { get; set; }
        public string CompanyType { get; set; }
        public string Geography { get; set; }

        public string Relationship { get; set; }
        public string Company { get; set; }
        public string Industry { get; set; }

        public string CompanyHeadcount { get; set; }
        public string Functions { get; set; }

        #endregion

        #region only company api part

        public string DepartmentSize { get; set; }
        public string DepartmentGrowth { get; set; }
        public string Fortune { get; set; }
        public string NumOfFollowers { get; set; }
        public string TechnologiesUsed { get; set; }
        public string JobOpportunities { get; set; }

        #endregion
    }
}