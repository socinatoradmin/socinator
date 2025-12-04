using System;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary
{
    public interface ITwitterRequestUrlProvider
    {
        string GetSearchForTagUrl(string queryType, string keyword, TdRequestParameters tdRequestParameter,
            string minPosition = null);
    }

    public class TwitterRequestUrlProvider : ITwitterRequestUrlProvider
    {
        public string GetSearchForTagUrl(string queryType, string keyword, TdRequestParameters tdRequestParameter,
            string minPosition = null)
        {
            if (minPosition == null || string.IsNullOrEmpty(minPosition))
            {
                if (queryType.Equals(TdUserInteractionQueryEnum.Keywords.ToString()))
                    return TdConstants.SearchUrl + Uri.EscapeDataString(keyword) + "&src=typd";

                if (queryType.Equals(TdUserInteractionQueryEnum.Hashtags.ToString()))
                    return TdConstants.SearchUrl + "%23" + Uri.EscapeDataString(keyword) + "&src=tyah";

                if (queryType.Equals(TdUserInteractionQueryEnum.LocationUsers.ToString()) ||
                    queryType.Equals(TdTweetInteractionQueryEnum.LocationTweets.ToString()) ||
                    queryType.Equals(EnumUtility.GetQueryFromEnum(TdUserInteractionQueryEnum.LocationUsers)) ||
                    queryType.Equals(EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.LocationTweets)))
                {
                    var SearchPattern = TdUtility.GetLocationWiseSearchFormat(keyword, out var refererPattern);
                    if (string.IsNullOrEmpty(SearchPattern))
                        return null;
                    tdRequestParameter.Referer =
                        TdConstants.MainUrl + "search?q=" + refererPattern + "&src=typd";
                    return TdConstants.MainUrl + "search?q=" + SearchPattern + "&src=typd";
                }

                if (queryType.Equals(TdUserInteractionQueryEnum.NearMyLocation.ToString()) ||
                    queryType.Equals(
                        EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.NearMyLocation)))
                {
                    tdRequestParameter.Referer =
                        TdConstants.SearchUrl + Uri.EscapeDataString(keyword) + "&src=typd";
                    return TdConstants.SearchUrl + Uri.EscapeDataString(keyword) + "&near=me&src=typd";
                }

                if (queryType.Equals(
                    EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SpecificUserTweets)))
                    return TdConstants.MainUrl + Uri.EscapeDataString(keyword);
            }

            else
            {
                if (queryType.Equals(TdUserInteractionQueryEnum.Keywords.ToString()))
                {
                    tdRequestParameter.Referer =
                        TdConstants.SearchUrl + Uri.EscapeDataString(keyword) + "&src=typd";
                    return TdConstants.SearchPaginationUrl + Uri.EscapeDataString(keyword) +
                           "&src=typd&include_available_features=1&include_entities=1&max_position=" +
                           minPosition + "&reset_error_state=false";
                }

                if (queryType.Equals(TdUserInteractionQueryEnum.Hashtags.ToString()))
                {
                    tdRequestParameter.Referer =
                        TdConstants.SearchUrl + "%23" + Uri.EscapeDataString(keyword) + "&src=tyah";
                    return TdConstants.SearchPaginationUrl + "%23" + Uri.EscapeDataString(keyword) +
                           "&src=tyah&include_available_features=1&include_entities=1&max_position=" +
                           minPosition + "&oldest_unread_id=0&reset_error_state=false";
                }

                if (queryType.Equals(TdUserInteractionQueryEnum.LocationUsers.ToString()) ||
                    queryType.Equals(TdTweetInteractionQueryEnum.LocationTweets.ToString()))
                {
                    var searchPattern = TdUtility.GetLocationWiseSearchFormat(keyword, out _);
                    tdRequestParameter.Referer =
                        TdConstants.MainUrl + "search?q=" + searchPattern + "&src=typd";
                    return TdConstants.SearchPaginationUrl + searchPattern +
                           "&src=typd&include_available_features=1&include_entities=1&max_position=" +
                           minPosition + "&reset_error_state=false";
                }

                if (queryType.Equals(TdUserInteractionQueryEnum.NearMyLocation.ToString()))
                {
                    tdRequestParameter.Referer =
                        TdConstants.SearchUrl + Uri.EscapeDataString(keyword) + "&near=me&src=typd";
                    return TdConstants.SearchPaginationUrl + Uri.EscapeDataString(keyword) +
                           "&near=me&src=typd&include_available_features=1&include_entities=1&max_position=" +
                           minPosition + "&reset_error_state=false";
                }

                if (queryType.Equals(
                    EnumUtility.GetQueryFromEnum(TdTweetInteractionQueryEnum.SpecificUserTweets)))
                {
                    tdRequestParameter.Referer = TdConstants.MainUrl + Uri.EscapeDataString(keyword);
                    return $"https://{TdConstants.Domain}/i/profiles/show/" +
                           $"{keyword}/timeline/tweets?include_available_features=1&include_entities=1&max_position={minPosition}&reset_error_state=false";
                }
            }

            throw new NotSupportedException($"{queryType} is not supported to search for tags");
        }
    }
}