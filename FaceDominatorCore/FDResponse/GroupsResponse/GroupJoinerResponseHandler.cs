using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;

namespace FaceDominatorCore.FDResponse.GroupsResponse
{
    public class GroupJoinerResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public bool IsRequestSent { get; set; }

        public GroupJoinerResponseHandler(IResponseParameter responseParameter, bool isQuestionsAsked)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            HtmlDocument objHtmlDocument = new HtmlDocument();

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            if (isQuestionsAsked)
                IsRequestSent = true;
            else
            {
                try
                {
                    objHtmlDocument.LoadHtml(decodedResponse);

                    if (objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 _55pi _2agf _4o_4 _p _4jy4 _517h _51sy mrm\"])") != null)
                        IsRequestSent = true;

                    else if (objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 button button _4jy3 _517h _51sy\"])") != null)
                    {
                        if (objHtmlDocument.DocumentNode.SelectNodes("(//a[@class=\"_42ft _4jy0 button button _4jy3 _517h _51sy\"])")[0].InnerHtml.Contains("/ajax/groups/confirm_cancel_join_dialog.php"))
                            IsRequestSent = true;
                    }
                    else if (decodedResponse.Contains("[\"HVmb9\"]"))
                        IsRequestSent = true;
                    else if (decodedResponse.Contains("\"type\":\"css\",\"src\":\""))
                        IsRequestSent = true;
                    else
                    {
                        IsRequestSent = false;
                        Status = false;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }
    }
}
