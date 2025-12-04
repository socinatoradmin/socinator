using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.LikeCommentsResponse
{

    public class CommentOnPostResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public CommentOnPostResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            if (!responseParameter.Response.Contains("errorSummary") && !responseParameter.Response.Contains("Not Found"))
            {
                if (responseParameter.Response.Contains("CommentAddedActive"))
                {
                    ObjFdScraperResponseParameters.IsCommentedOnPost = true;
                    Status = true;
                    try
                    {
                        ObjFdScraperResponseParameters.CommentId = Regex.Matches(responseParameter.Response, "fbid\":\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString();
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorLog(ex.Message);
                    }
                }
                else if (responseParameter.Response.Contains("legacy_fbid\": \""))
                {
                    Status = true;
                    ObjFdScraperResponseParameters.CommentId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "legacy_fbid\": \"(.*?)\"");
                }
            }
            else if (responseParameter.Response.Contains("temporarily blocked from making public comments"))
                ObjFdScraperResponseParameters.IsBlocked = true;

        }

        public CommentOnPostResponseHandler(IResponseParameter responseParameter, bool isBrowser)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
            {
                Status = false;
                return;
            }
            else if (string.IsNullOrEmpty(responseParameter.Response))
            {
                ObjFdScraperResponseParameters.IsCommentedOnPost = false;
                Status = false;
                return;

            }

            ObjFdScraperResponseParameters.IsCommentedOnPost = true;
            Status = true;
            try
            {
                ObjFdScraperResponseParameters.CommentId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "{\"id\":\"(.*?)\"");

                if (!FdFunctions.IsIntegerOnly(ObjFdScraperResponseParameters.CommentId))
                {
                    ObjFdScraperResponseParameters.CommentId = Regex.Split(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ObjFdScraperResponseParameters.CommentId)), "_")[1];
                }
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }

        }
    }
}
