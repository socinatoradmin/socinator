using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using Unity;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary
{
    public interface IContentUploaderService
    {
        string UploadMediaContent(DominatorAccountModel twitterAccount, params string[] listFilePath);
    }

    public class ContentUploaderService : IContentUploaderService
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IDateProvider _dateProvider;
        private readonly IDelayService _delayService;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IWebService _webService;

        public ContentUploaderService(IWebService webService, IDateProvider dateProvider,
            IFileSystemProvider fileSystemProvider, IAccountScopeFactory accountScopeFactory,
            IDelayService delayService)
        {
            _webService = webService;
            _dateProvider = dateProvider;
            _fileSystemProvider = fileSystemProvider;
            _accountScopeFactory = accountScopeFactory;
            _delayService = delayService;
        }

        public string UploadMediaContent(DominatorAccountModel twitterAccount, params string[] listFilePath)
        {
            try
            {
                var httpHelper = _accountScopeFactory[twitterAccount.AccountId]
                    .Resolve<ITdHttpHelper>();
                var tdRequestParameter = (TdRequestParameters) httpHelper.GetRequestParameter();

                if (httpHelper.GetRequestParameter().Cookies == null ||
                    httpHelper.GetRequestParameter().Cookies.Count == 0)
                    httpHelper.GetRequestParameter().Cookies = twitterAccount.Cookies;
                var account = new AccountModel(twitterAccount);
                if (listFilePath.Length > 4)
                    GlobusLogHelper.log.Info(Log.CustomMessage, twitterAccount.AccountBaseModel.AccountNetwork,
                        twitterAccount.UserName, ActivityType.Tweet,
                        "LangKeyCannotTweetMoreThanFourImages".FromResourceDictionary());

                var mediaId = new List<string>();
                var timeStampId = (TdUtility.UnixTimestampFromDateTime(_dateProvider.UtcNow()) * 1000).ToString();
                foreach (var path in listFilePath.Where(a => !string.IsNullOrEmpty(a)).Take(4))
                {
                    var content = GetContent(path);
                    content = TdUtility.EncodeBase64String(content);
                    tdRequestParameter.SetupHeaders(Path:TdConstants.UploadUrl,Method:"POST");
                    var postData = "authenticity_token=" + account.postAuthenticityToken + "&iframe_callback=&media=" +
                                   content + "&upload_id=" + timeStampId + "&origin=https%3A%2F%2Ftwitter.com";
                    var response = httpHelper.PostRequest(TdConstants.UploadUrl, postData).Response;
                    var tempMediaId = Utilities.GetBetween(response, "media_id\\\":", ",");
                    mediaId.Add(tempMediaId);
                    _delayService.ThreadSleep(TdConstants.ConsecutiveGetReqInteval);
                }

                var finalMediaIds = string.Join(",", mediaId);

                return finalMediaIds;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
                return null;
            }
        }

        private string GetContent(string filePath)
        {
            string content;

            if (filePath.Contains("https:") || filePath.Contains("http:"))
                content = Convert.ToBase64String(_webService.GetImageBytesFromUrl(filePath));

            else
                content = Convert.ToBase64String(_fileSystemProvider.ReadAllBytes(filePath));
            return content;
        }
    }
}