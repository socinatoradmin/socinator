using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class CheckJoiningStatusResponseHandler : FdResponseHandler
    {
        public string JoiningStatus { get; set; }

        public bool IsMember { get; set; }

        public bool IsQuestionsAsked { get; set; }

        public bool IsQuestionsAnswered { get; set; }

        public bool IsIncompleteSource { get; set; }

        public string GroupId { get; set; }


        public CheckJoiningStatusResponseHandler(IResponseParameter responseParameter) :
            base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response).Replace("<!--", string.Empty).Replace("--!>", string.Empty);

            objHtmlDocument.LoadHtml(decodedResponse);

            if (decodedResponse.Contains("groupID:"))
            {
                GroupId = Regex.Matches(decodedResponse, "groupID:\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();
            }

            HtmlNodeCollection objJoiningStatusNode = objHtmlDocument.DocumentNode.SelectNodes("(//div[@id=\"headerArea\"])");

            if (objJoiningStatusNode == null)
            {
                if (decodedResponse.Contains("entity_id\":"))
                    GroupId = FdRegexUtility.FirstMatchExtractor(decodedResponse, FdConstants.EntityIdRegex);

                IsIncompleteSource = true;
                return;
            }

            objHtmlDocument.LoadHtml(objJoiningStatusNode[0].InnerHtml);

            objJoiningStatusNode = objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 _55pi _2agf _4o_4 _p _4jy4 _517h _51sy\"])");

            if (objJoiningStatusNode != null)
            {
                JoiningStatus = "Joined";
                IsMember = true;
            }
            else if (objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 _55pi _2agf _4o_4 _p _4jy4 _517h _51sy mrm\"])") != null)
            {
                try
                {
                    JoiningStatus = "Pending";
                    IsMember = false;

                    var editAnswerSection = string.Empty;

                    if (decodedResponse.Contains("<code id=\"u_0_1u\""))
                        editAnswerSection = Regex.Split(decodedResponse, "code id=\"u_0_1u\"")[1];
                    else if (decodedResponse.Contains("<code id=\"u_0_1w\""))
                        editAnswerSection = Regex.Split(decodedResponse, "code id=\"u_0_1w\"")[1];

                    objHtmlDocument.LoadHtml(editAnswerSection);

                    objJoiningStatusNode = objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 _2pib _4jy3 _517h _51sy\"])");

                    if (objJoiningStatusNode != null)
                    {
                        var membershipCriteria = objJoiningStatusNode[0].OuterHtml;

                        if (membershipCriteria.Contains("membership_criteria_answer"))
                        {
                            IsQuestionsAsked = true;
                            IsQuestionsAnswered = true;
                        }


                    }

                    objJoiningStatusNode = objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 _2pib _4jy3 _4jy1 selected _51sy\"])");

                    if (objJoiningStatusNode != null)
                    {
                        var membershipCriteria = objJoiningStatusNode[0].OuterHtml;

                        if (membershipCriteria.Contains("membership_criteria_answer"))
                        {
                            IsQuestionsAsked = true;
                            IsQuestionsAnswered = false;
                        }
                    }

                }
                catch (Exception ex)
                {
                    ex.ErrorLog(ex.Message);
                }
            }
            else
            {
                try
                {
                    JoiningStatus = "Join Group";
                    IsMember = false;

                    objJoiningStatusNode = objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 _21ku _4jy4 _4jy1 selected _51sy mrm\"])");

                    var membershipCriteria = objJoiningStatusNode[0].OuterHtml;

                    IsQuestionsAsked = membershipCriteria.Contains("membership_criteria_answer");

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

        }
    }
}
