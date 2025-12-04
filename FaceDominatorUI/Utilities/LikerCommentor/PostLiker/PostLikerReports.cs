using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Campaigns;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FdReports;
using FaceDominatorCore.Interface;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace FaceDominatorUI.Utilities.LikerCommentor.PostLiker
{
    public class PostLikerReports : IFdReportFactory
    {
        public static List<string> DataAccount = new List<string>();

        public static List<string> DataCampaign = new List<string>();

        private readonly string _activityType = ActivityType.PostLiker.ToString();

        public List<PostReportAccountModel> AccountsInteractedPosts = new List<PostReportAccountModel>();

        public ObservableCollection<PostReportModel> InteractedPostModel = new ObservableCollection<PostReportModel>();
        public string Header { get; set; } = string.Empty;

        public ObservableCollection<QueryInfo> GetSavedQuery(ActivityType subModuleName, string activitySettings)
        {
            return JsonConvert.DeserializeObject<PostLikerCommentorModel>(activitySettings).SavedQueries;
        }


        ObservableCollection<object> IFdReportFactory.GetCampaignsReport(ReportModel reportModel,
            List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails)
        {
            var dataBase = new DbCampaignService(campaignDetails.CampaignId);

            InteractedPostModel.Clear();
            DataCampaign.Clear();

            #region get data from InteractedUsers table and add to FollowerReportModel

            dataBase.GetAllInteractedData<InteractedPosts>().ForEach(
                report =>
                {
                    try
                    {
                        var post = JsonConvert.DeserializeObject<FacebookPostDetails>(report.PostDescription);

                        if (InteractedPostModel.FirstOrDefault(x =>
                                x.PostId == post.Id && x.ReactedAsPageUrl == post.LikePostAsPageId) == null)
                        {
                            if (post.OtherMediaUrl.Count() > 32700)
                                post.OtherMediaUrl = post.OtherMediaUrl.Remove(32700);
                            InteractedPostModel.Add(new PostReportModel
                            {
                                Id = report.Id,
                                AccountEmail = report.AccountEmail,
                                QueryType = report.QueryType,
                                QueryValue = report.QueryValue,
                                PostId = post.Id,
                                PostUrl = FdConstants.FbHomeUrl + report.PostId,
                                LikeType = report.LikeType,
                                IsReactedAsPage = !string.IsNullOrEmpty(post.LikePostAsPageId),
                                ReactedAsPageUrl = FdConstants.FbHomeUrl + post.LikePostAsPageId,
                                Likes = post.LikersCount,
                                Comments = post.CommentorCount,
                                Shares = post.SharerCount,
                                PostDescription = post.Caption?.Replace(",", string.Empty)?.Replace("\r\n", " "),
                                SubDescription = post.SubDescription?.Replace(",", string.Empty)?.Replace("\r\n", " "),
                                PostedDateTime = post.PostedDateTime,
                                PostTitle = post.Title?.Replace(",", string.Empty)?.Replace("\r\n", " "),
                                PostedBy = post.OwnerName?.Replace(",", string.Empty)?.Replace("\r\n", " "),
                                OwnerId = post.OwnerId,
                                MediaType = post.MediaType,
                                MediaUrl = post.MediaUrl,
                                OtherMediaUrl = post.OtherMediaUrl?.Replace(",", string.Empty)?.Replace("\r\n", " "),
                                NavigationUrl = post.NavigationUrl,
                                InteractionTimeStamp = report.InteractionDateTime
                            });

                            post.LikePostAsPageId = string.IsNullOrEmpty(post.LikePostAsPageId)
                                ? string.Empty
                                : post.LikePostAsPageId;

                            if (DataCampaign.FirstOrDefault(x =>
                                    x.Contains(report.PostId) && x.Contains(report.AccountEmail) &
                                    x.Contains(post.LikePostAsPageId)) == null)
                            {
                                var isLikedCommentedAsPage = !string.IsNullOrEmpty(post.LikePostAsPageId);

                                var likeAsPageUrl = isLikedCommentedAsPage
                                    ? $"{FdConstants.FbHomeUrl + post.LikePostAsPageId}"
                                    : string.Empty;

                                DataCampaign.Add(report?.AccountEmail + "," + report?.QueryType + ","
                                                 + report?.QueryValue + ","
                                                 + report?.PostId + ","
                                                 + $"{FdConstants.FbHomeUrl}{report?.PostId}" + ","
                                                 + $"\"{report?.LikeType}\"" + ","
                                                 + $"{isLikedCommentedAsPage}" + ","
                                                 + likeAsPageUrl + ","
                                                 + post?.LikersCount + ","
                                                 + post?.CommentorCount + ","
                                                 + post?.SharerCount + ","
                                                 + $"\"{post?.Caption?.Replace("\r\n", " ")?.Replace("\"", string.Empty)}\"" +
                                                 ","
                                                 + $"\"{post?.SubDescription?.Replace("\r\n", " ")?.Replace("\"", string.Empty)}\"" +
                                                 ","
                                                 + post?.PostedDateTime + ","
                                                 + $"\"{post?.Title?.Replace("\r\n", " ")?.Replace("\"", string.Empty)}\"" +
                                                 ","
                                                 + post?.PostedBy + ","
                                                 + post?.OwnerName + ","
                                                 + post?.OwnerId + ","
                                                 + post?.MediaType + ","
                                                 + post?.MediaUrl + ","
                                                 + post?.OtherMediaUrl?.Trim()
                                                                        ?.Replace(",", string.Empty)
                                                                        ?.Replace("\r\n", " ")
                                                                        ?.Replace("\"", string.Empty) + ","
                                                 + post?.NavigationUrl + ","
                                                 + report?.InteractionDateTime);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

            #endregion

            #region Generate Reports column with data

            //campaign.SelectedAccountList.ToList().ForEach(x =>
            //{
            reportModel.GridViewColumn =
                new ObservableCollection<GridViewColumnDescriptor>
                {
                    new GridViewColumnDescriptor {ColumnHeaderText = "ID", ColumnBindingText = "Id"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText =
                            $"{"LangKeyAccount".FromResourceDictionary()} {"LangKeyEmail".FromResourceDictionary()}",
                        ColumnBindingText = "AccountEmail"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyQueryType".FromResourceDictionary(), ColumnBindingText = "QueryType"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyQueryValue".FromResourceDictionary(),
                        ColumnBindingText = "QueryValue"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostId".FromResourceDictionary(), ColumnBindingText = "PostId"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostUrl".FromResourceDictionary(), ColumnBindingText = "PostUrl"},
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyLikeType".FromResourceDictionary(), ColumnBindingText = "LikeType"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyIsLikedAsPage".FromResourceDictionary(),
                        ColumnBindingText = "IsReactedAsPage"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyLikedAsPageUrl".FromResourceDictionary(),
                        ColumnBindingText = "ReactedAsPageUrl"
                    },

                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyLikeCount".FromResourceDictionary(), ColumnBindingText = "Likes"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyCommentCount".FromResourceDictionary(),
                        ColumnBindingText = "Comments"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyShareCount".FromResourceDictionary(), ColumnBindingText = "Shares"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostDescription".FromResourceDictionary(),
                        ColumnBindingText = "PostDescription"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeySubDescription".FromResourceDictionary(),
                        ColumnBindingText = "SubDescription"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostedDateTime".FromResourceDictionary(),
                        ColumnBindingText = "PostedDateTime"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyPostTitle".FromResourceDictionary(), ColumnBindingText = "PostTitle"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyPostedBy".FromResourceDictionary(), ColumnBindingText = "PostedBy"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyMediaType".FromResourceDictionary(), ColumnBindingText = "MediaType"
                    },
                    new GridViewColumnDescriptor
                        {ColumnHeaderText = "LangKeyMediaUrl".FromResourceDictionary(), ColumnBindingText = "MediaUrl"},
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LanKeyOtherMediaUrl".FromResourceDictionary(),
                        ColumnBindingText = "OtherMediaUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyNavigationUrl".FromResourceDictionary(),
                        ColumnBindingText = "NavigationUrl"
                    },
                    new GridViewColumnDescriptor
                    {
                        ColumnHeaderText = "LangKeyDate".FromResourceDictionary(),
                        ColumnBindingText = "InteractionTimeStamp"
                    }
                };
            //});

            // reportModel.ReportCollection = CollectionViewSource.GetDefaultView(InteractedPostModel);

            #endregion

            return new ObservableCollection<object>(InteractedPostModel);
        }

        public IList GetsAccountReport(DbAccountService dataBase)
        {
            IList reportDetails = dataBase
                .Get<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts>(x =>
                    x.ActivityType == _activityType).ToList();

            AccountsInteractedPosts.Clear();
            DataAccount.Clear();

            var id = 1;

            foreach (DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts report in reportDetails)
                try
                {
                    FacebookPostDetails post;

                    try
                    {
                        post = JsonConvert.DeserializeObject<FacebookPostDetails>(report.PostDescription);
                    }
                    catch (Exception ex)
                    {
                        post = new FacebookPostDetails();
                        ex.DebugLog();
                    }

                    if (AccountsInteractedPosts.FirstOrDefault(x =>
                            x.PostId == post.Id && x.ReactedAsPageUrl == post.LikePostAsPageId) == null)
                    {
                        AccountsInteractedPosts.Add(
                            new PostReportAccountModel
                            {
                                Id = id,
                                QueryType = report.QueryType,
                                QueryValue = report.QueryValue,
                                ActivityType = ActivityType.PostLiker.ToString(),
                                PostUrl = FdConstants.FbHomeUrl + report.PostId,
                                LikeType = report.LikeType,
                                IsReactedAsPage = !string.IsNullOrEmpty(post.LikePostAsPageId),
                                ReactedAsPageUrl = FdConstants.FbHomeUrl + post.LikePostAsPageId,
                                Likes = post.LikersCount,
                                Comments = post.CommentorCount,
                                Shares = post.SharerCount,
                                PostedDateTime = post.PostedDateTime,
                                PostTitle = post.Title.Replace(",", string.Empty).Replace("\r\n", " "),
                                PostDescription = post.Caption.Replace(",", string.Empty).Replace("\r\n", " "),
                                SubDescription = post.SubDescription.Replace(",", string.Empty).Replace("\r\n", " "),
                                PostedBy = post.OwnerName.Replace(",", string.Empty).Replace("\r\n", " "),
                                OwnerId = post.OwnerId,
                                MediaType = post.MediaType,
                                MediaUrl = post.MediaUrl,
                                OtherMediaUrl = post.OtherMediaUrl.Replace(",", string.Empty).Replace("\r\n", " "),
                                NavigationUrl = post.NavigationUrl,
                                InteractionTimeStamp = report.InteractionDateTime
                            }
                        );
                        post.LikePostAsPageId = string.IsNullOrEmpty(post.LikePostAsPageId)
                            ? string.Empty
                            : post.LikePostAsPageId;


                        if (DataAccount.FirstOrDefault(x =>
                                x.Contains(report.PostId) && x.Contains(report.QueryType) &&
                                x.Contains(post.LikePostAsPageId)) == null)
                        {
                            var isLikedCommentedAsPage = !string.IsNullOrEmpty(post.LikePostAsPageId);

                            var likeAsPageUrl = isLikedCommentedAsPage
                                ? $"{FdConstants.FbHomeUrl + post.LikePostAsPageId}"
                                : string.Empty;

                            DataAccount.Add(report.QueryType + ","
                                                             + report.QueryValue + ","
                                                             + report.PostId + ","
                                                             + $"{FdConstants.FbHomeUrl}{report.PostId}" + ","
                                                             + $"\"{report.LikeType}\"" + ","
                                                             + $"{isLikedCommentedAsPage}" + ","
                                                             + likeAsPageUrl + ","
                                                             + post.LikersCount + ","
                                                             + post.CommentorCount + ","
                                                             + post.SharerCount + ","
                                                             + $"\"{post.Caption.Replace("\r\n", " ").Replace("\"", string.Empty)}\"" +
                                                             ","
                                                             + $"\"{post.SubDescription.Replace("\r\n", " ").Replace("\"", string.Empty)}\"" +
                                                             ","
                                                             + post.PostedDateTime + ","
                                                             + $"\"{post.Title.Replace("\r\n", " ").Replace("\"", string.Empty)}\"" +
                                                             ","
                                                             + post.OwnerName + ","
                                                             + post.OwnerId + ","
                                                             + post.MediaType + ","
                                                             + post.MediaUrl + ","
                                                             + $"\"{post.OtherMediaUrl.Replace("\r\n", " ").Replace("\"", string.Empty)}\"" +
                                                             ","
                                                             + post.NavigationUrl + ","
                                                             + report.InteractionDateTime);
                        }
                    }

                    id++;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            return AccountsInteractedPosts;
        }

        public void ExportReports(ActivityType activityType, string fileName, ReportType reportType)
        {
            var csvData = new List<string>();

            #region Campaign reports

            if (reportType == ReportType.Campaign)
            {
                //Header = "AccountEmail,QueryType,QueryValue,Post Id,Post Url,Like Type,Is Liked As Page, Liked as Page Url,Like Count,Comment Count,Share  Count,Post Description,SubDescription,Posted DateTime,Post Title, Owner Name ,Owner Id, MediaType , Media Url ,Other MediaUrl,Navigation Url , Date";
                Header = PostsReportHeader();
                DataCampaign.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            #region Account reports

            if (reportType == ReportType.Account)
            {
                //Header = "QueryType,QueryValue,Post Id,Post Url,Like Type,Is Liked As Page, Liked as Page Url,Like Count,Comment Count,Share  Count,Post Description,SubDescription,Posted DateTime,Post Title, Owner Name ,Owner Id , MediaType , Media Url ,Other MediaUrl,Navigation Url , Date";
                Header = PostsReportHeader(false);
                DataAccount.ToList().ForEach(report =>
                {
                    try
                    {
                        csvData.Add(report);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
            }

            #endregion

            if (csvData.Count == 0)
            {
                Dialog.ShowDialog(Application.Current.MainWindow,
                    "LangKeyWarning".FromResourceDictionary(), "LangKeyReportIsNotAvailable".FromResourceDictionary());
                return;
            }


            DominatorHouseCore.Utility.Utilities.ExportReports(fileName, Header, csvData);
        }

        public string PostsReportHeader(bool addAccount = true)
        {
            var listResource = new List<string>();
            if (addAccount)
                listResource.Add("LangKeyAccount");
            listResource.Add("LangKeyQueryType");
            listResource.Add("LangKeyQueryValue");
            listResource.Add("LangKeyPostId");
            listResource.Add("LangKeyPostUrl");
            listResource.Add("LangKeyLikeType");
            listResource.Add("LangKeyIsLikedAsPage");
            listResource.Add("LangKeyLikedAsPageUrl");
            listResource.Add("LangKeyLikeCount");
            listResource.Add("LangKeyCommentCount");
            listResource.Add("LangKeyShareCount");
            listResource.Add("LangKeyPostDescription");
            listResource.Add("LangKeySubDescription");
            listResource.Add("LangKeyPostedDateTime");
            listResource.Add("LangKeyPostTitle");
            listResource.Add("LangKeyPostedBy");
            listResource.Add("LangKeyOwnerName");
            listResource.Add("LangKeyOwnerId");
            listResource.Add("LangKeyMediaType");
            listResource.Add("LangKeyMediaUrl");
            listResource.Add("LanKeyOtherMediaUrl");
            listResource.Add("LangKeyNavigationUrl");
            listResource.Add("LangKeyDate");

            return listResource.ReportHeaderFromResourceDict();
        }
    }
}