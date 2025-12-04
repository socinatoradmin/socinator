using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using HtmlAgilityPack;
using System;
using System.Linq;

namespace FaceDominatorCore.FDResponse.CommonResponse
{

    public class ScrapPostFromAlbumsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public ScrapPostFromAlbumsResponseHandler(IResponseParameter responseParameter, string albumName)
            : base(responseParameter)
        {

            if (responseParameter.HasError || string.IsNullOrEmpty(responseParameter.Response))
                return;

            ObjFdScraperResponseParameters = new FdScraperResponseParameters { AlbumName = albumName };

            var decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetPostListFromResponse(decodedResponse);

            GetPageletData(decodedResponse);

            if (ObjFdScraperResponseParameters.ListPostDetails.Count > 0)
            {
                Status = true;

                HasMoreResults = ObjFdScraperResponseParameters.ListPostDetails.Count == 12;
            }

        }

        private void GetPageletData(string decodedResponse)
        {
            ObjFdScraperResponseParameters.AjaxToken = FdRegexUtility.FirstMatchExtractor(decodedResponse, "\"token\":\"(.*?)\"");
            // ObjFdScraperResponseParameters.ImpressionSource = FdRegexUtility.FirstMatchExtractor(decodedResponse, "impression_source=(.*?)\"");
            //  PageletData = FdRegexUtility.FirstMatchExtractor(decodedResponse, "cursor=(.*?)&");
            ObjFdScraperResponseParameters.FinalEncodedQuery = FdRegexUtility.FirstMatchExtractor(decodedResponse, "encrypted\":\"(.*?)\"");
        }

        private void GetPostListFromResponse(string decodedResponse)
        {
            try
            {
                HtmlDocument objDocument = new HtmlDocument();

                objDocument.LoadHtml(decodedResponse);

                var albumNameDetail = objDocument.DocumentNode.SelectNodes("//div[@class=\"_52jf _5vbf\"]");

                var nodeCollection = objDocument.DocumentNode.SelectNodes("//a[@class='_39pi _1mh- _4i6j']");

                if (albumNameDetail != null)
                    ObjFdScraperResponseParameters.AlbumName = albumNameDetail[0].InnerHtml;

                //imgsrc

                ObjFdScraperResponseParameters.ListPostDetails = (from node in nodeCollection
                                                                  let postUrl = FdRegexUtility.FirstMatchExtractor(node.OuterHtml, "href=\"/(.*?)\"")
                                                                  let postId = FdRegexUtility.FirstMatchExtractor(postUrl, "fbid=(.*?)&")
                                                                  where !string.IsNullOrEmpty(postId)
                                                                  select new FacebookPostDetails
                                                                  {
                                                                      PostUrl = FdConstants.FbHomeUrl + postUrl,
                                                                      AlbumName = ObjFdScraperResponseParameters.AlbumName,
                                                                      Id = FdRegexUtility.FirstMatchExtractor(postUrl, "fbid=(.*?)&")
                                                                  }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
