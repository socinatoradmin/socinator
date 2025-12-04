using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class NewsFeedPaginationResonseHandler : FdResponseHandler, IResponseHandler
    {

        public bool Status { get; set; }

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public NewsFeedPaginationResonseHandler(IResponseParameter responseParameter, bool isPagination
            , string finalQuery) : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (isPagination)
            {
                ObjFdScraperResponseParameters.IsPagination = true;
                ObjFdScraperResponseParameters.FinalEncodedQuery = finalQuery;
            }


            try
            {
                string[] cursorDataArray = Regex.Split(responseParameter.Response, "BrowseScrollingPager");

                string cursorData = string.Empty;
                if (!ObjFdScraperResponseParameters.IsPagination)
                {
                    if (cursorDataArray.Length > 2)
                    {
                        cursorData = Regex.Split(cursorDataArray[2], "{cursor:(.*?),display_params", RegexOptions.Singleline)[1];
                        cursorData = cursorData.Replace(",", ",\"").Replace(":", "\":");
                    }
                    GetEncodedQuery(responseParameter);
                }
                else
                {
                    if (cursorDataArray.Length > 1)
                        cursorData =
                            Regex.Split(cursorDataArray[1], "{\"cursor\":(.*?),\"display_params", RegexOptions.Singleline).Length > 1
                                ? Regex.Split(cursorDataArray[1], "{\"cursor\":(.*?),\"display_params", RegexOptions.Singleline)[1]
                                : string.Empty;
                }

                if (string.IsNullOrEmpty(cursorData))
                    PageletData = string.Empty;
                else
                {
                    var fullQueryParameters = $"{ObjFdScraperResponseParameters.FinalEncodedQuery}{cursorData}}}";
                    PageletData = fullQueryParameters;
                }

            }
            catch (ArgumentException)
            {

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

        }

        private void GetEncodedQuery(IResponseParameter responseParameter)
        {
            try
            {

                var encodedQuery = Regex.Split(responseParameter.Response, "{encoded_query:\"(.*?)encoded_query_no_rewrite", RegexOptions.Singleline)[1];

                encodedQuery = "{\"view\":\"list\",\"encoded_query\":\"" + encodedQuery + "\"encoded_title\":";

                var encodedTitle = Regex.Split(responseParameter.Response, "encoded_title:(.*?)}},", RegexOptions.Singleline)[1];

                ObjFdScraperResponseParameters.FinalEncodedQuery = $"{encodedQuery}{encodedTitle}" + "},\"cursor\":";

            }
            catch (ArgumentException)
            {

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}
