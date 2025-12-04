namespace DominatorHouseCore.Utility
{
    public static class Log
    {
        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        /// </summary>
        public static string StartingJob { get; set; } = "{0}\t {1}\t {2}\t" +
                                                         "LangKeyStartedJobTo".FromResourceDictionary() + " {2} \t" +
                                                         CodeConstants.StartedJob;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        /// </summary>
        public static string JobCompleted { get; set; } = "{0}\t {1}\t {2}\t" +
                                                          "LangKeySuccessfullyCompleteJobTo".FromResourceDictionary() +
                                                          " {2}. \t" + CodeConstants.CompletedJob;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string AccountLogin { get; set; } = "{0}\t {1}\t Login \t " +
                                                          "LangKeyAttemptToLogin".FromResourceDictionary() + "\t" +
                                                          CodeConstants.LoginAttempt;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string SuccessfulLogin { get; set; } =
            "{0}\t {1}\t " + "LangKeyLoginSuccess".FromResourceDictionary() + " \t " +
            "LangKeyLoginSuccessful".FromResourceDictionary() + "\t" + CodeConstants.LoginSuccessful;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        /// </summary>
        public static string LoginFailed { get; set; } = "{0}\t {1}\t " +
                                                         "LangKeyLoginFailed".FromResourceDictionary() + " \t " +
                                                         "LangKeyLoginFailedWithError".FromResourceDictionary() +
                                                         " {2}.\t" + CodeConstants.LoginFailed;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        ///     3 = ObjectId
        /// </summary>
        public static string StartedActivity { get; set; } = "{0}\t {1}\t {2}\t " +
                                                             "LangKeyTryingTo".FromResourceDictionary() + " {2} {3}\t" +
                                                             CodeConstants.StartedActivity;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        ///     3 = ObjectId
        /// </summary>
        public static string ActivitySuccessful { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeySuccessfulTo".FromResourceDictionary() + " {2} {3}\t" +
            CodeConstants.ActivitySuccessful;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        ///     3 = ObjectId
        ///     4 = SocialNetworkError
        /// </summary>
        public static string ActivityFailed { get; set; } = "{0}\t {1}\t {2}\t " +
                                                            "LangKeyFailedTo".FromResourceDictionary() + " {2} " +
                                                            "LangKeyWithError".FromResourceDictionary() + " {3}\t" +
                                                            CodeConstants.ActivityFailed;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = CampaignName
        /// </summary>
        public static string CampaignDeleted { get; set; } =
            "{0}\t {1}\t " + "LangKeyCampaign".FromResourceDictionary() + "\t " +
            "LangKeySuccessfullyDeleted".FromResourceDictionary() + "\t" + CodeConstants.CampaignDeleted;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = CampaignName
        /// </summary>
        public static string CampaignPaused { get; set; } =
            "{0}\t {1}\t " + "LangKeyCampaign".FromResourceDictionary() + "\t " +
            "LangKeySuccessfullyPaused".FromResourceDictionary() + "\t" + CodeConstants.CampaignPaused;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = CampaignName
        /// </summary>
        public static string ActivatedCampaign { get; set; } =
            "{0}\t {1}\t " + "LangKeyCampaign".FromResourceDictionary() + " \t " +
            "LangKeyActivatedSuccessfully".FromResourceDictionary() + "\t" + CodeConstants.ActivatedCampaign;


        public static string UpdatingDetails { get; set; } = "{0}\t {1}\t {2}\t " +
                                                             "LangKeyStarted".FromResourceDictionary() + " {2}" +
                                                             "LangKeySynchronization".FromResourceDictionary() + "\t" +
                                                             CodeConstants.UpdatingDetails;

        public static string DetailsUpdated { get; set; } = "{0}\t {1}\t {2}\t " +
                                                            "LangKeySynchronization".FromResourceDictionary() +
                                                            " {2} " + "LangKeySuccessful".FromResourceDictionary() +
                                                            "\t" + CodeConstants.DetailsUpdated;

        public static string UploadedAccount { get; set; } = "{0}\t {1}\t {2}\t " +
                                                             "LangKeySuccessfullyAdded".FromResourceDictionary() + " " +
                                                             "LangKeyAccountTo".FromResourceDictionary() + " {2}.\t" +
                                                             CodeConstants.UploadedAccount;

        public static string SelectedAccount { get; set; } = "{0}\t {1}\t " +
                                                             "LangKeyAccountSelection".FromResourceDictionary() +
                                                             "\t " +
                                                             "LangKeySuccessfullyAdded".FromResourceDictionary() +
                                                             " {2} " + "LangKeyAccountTo".FromResourceDictionary() +
                                                             " {3}.\t" + CodeConstants.SelectedAccount;

        /// <summary>
        ///     0 = NumberOfAccounts
        ///     1 = PlatformName
        /// </summary>
        public static string DeletedAccounts { get; set; } = "{0}\t {1}\t {2}\t " +
                                                             "LangKeyDeleted".FromResourceDictionary() + " " +
                                                             "LangKeyAccountsFrom".FromResourceDictionary() +
                                                             " {3}.\t" + CodeConstants.DeletedAccounts;

        /// <summary>
        ///     0 = account.SocialNetwork
        ///     1 = account.Username
        /// </summary>
        public static string AccountEdited { get; set; } =
            "{0}\t {1}\t " + "LangKeyAccountInfo".FromResourceDictionary() + "\t " +
            "LangKeyDetailsUpdatedSuccessfully".FromResourceDictionary() + "\t" + CodeConstants.AccountEdited;

        /// <summary>
        ///     0 = account.SocialNetwork
        ///     1 = account.Username
        ///     2 = ActivityType
        ///     3 = DelaySeconds
        /// </summary>
        public static string DelayBetweenActivity { get; set; } = "{0}\t {1}\t {2}\t " +
                                                                  "LangKeyNextOperationTo".FromResourceDictionary() +
                                                                  " {2} " +
                                                                  "LangKeyWillPerformIn".FromResourceDictionary() +
                                                                  " {3} " + "LangKeySeconds".FromResourceDictionary() +
                                                                  "\t" + CodeConstants.DelayBetweenActivity;

        public static string NextJobExpectedToStartBy { get; set; } = "{0}\t {1}\t {2}\t " +
                                                                      "LangKeyNextJobTo".FromResourceDictionary() +
                                                                      " {2} " + "LangKeyIsExpectedToStartBy"
                                                                          .FromResourceDictionary() + " {3}\t" +
                                                                      CodeConstants.NextJobExpectedToStartBy;

        public static string JobLimitReached { get; set; } = "{0}\t {1}\t {2}\t " +
                                                             "LangKeyHasReachedPerJobLimitOf".FromResourceDictionary() +
                                                             " {3}. \t" + CodeConstants.JobLimitReached;

        public static string DailyLimitReached { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerDayLimitOf".FromResourceDictionary() + " {3}. \t" +
            CodeConstants.DailyLimitReached;

        public static string HourlyLimitReached { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerHourLimitOf".FromResourceDictionary() + " {3}. \t" +
            CodeConstants.HourlyLimitReached;

        public static string WeeklyLimitReached { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerWeekLimitOf".FromResourceDictionary() + " {3}. \t" +
            CodeConstants.WeeklyLimitReached;

        public static string LimitReached { get; set; } = "{0}\t {1}\t {2}\t " +
                                                          "LangKeyHasReached".FromResourceDictionary() + " {2} " +
                                                          "LangKeyLimitOf".FromResourceDictionary() + " {3}\t" +
                                                          CodeConstants.LimitReached;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        /// </summary>
        public static string OtherConfigurationStarted { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeyOtherConfigurationFor".FromResourceDictionary() + " {2} " +
            "LangKeyIsStarted".FromResourceDictionary() + "\t" + CodeConstants.OtherConfigurationStarted;

        public static string OtherConfigurationCompleted { get; set; } = "{0}\t {1}\t {2}\t " +
                                                                         "LangKeyOtherConfigurationFor"
                                                                             .FromResourceDictionary() + " {2} " +
                                                                         "LangKeyIsCompleted".FromResourceDictionary() +
                                                                         "\t" + CodeConstants
                                                                             .OtherCongigurationCompleted;

        public static string FilterApplied { get; set; } = "{0}\t {1}\t {2}\t " +
                                                           "LangKeyAppliedFilterTo".FromResourceDictionary() + " {2} " +
                                                           "LangKeySearchResults".FromResourceDictionary() + " {3}\t" +
                                                           CodeConstants.FilterApplied;

        public static string DetailsScraped { get; set; } = "{0}\t {1}\t {2}\t " +
                                                            "LangKeyFound".FromResourceDictionary() + " {2} " +
                                                            "LangKeyScrapetypeToActivity".FromResourceDictionary() +
                                                            " {3}\t" + CodeConstants.DetailsScraped;

        public static string NoMoreDataToPerform { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeyNoMoreDataAvailableToPerform".FromResourceDictionary() + "\t" +
            CodeConstants.NoMoreDataToPerform;
        public static string CanNotAccessToThisAccout { get; set; } = "{0}\t{1}\t{2}\t" + "LangKeyCannotAccessToThisAccount".FromResourceDictionary() + " {3}" + "\t" +
                                                                       CodeConstants.CanNotAccessToThisAccout;
        public static string FoundXResults { get; set; } = "{0}\t {1}\t {5}\t" +
                                                           "LangKeyFound".FromResourceDictionary() + " {2} " +
                                                           "LangKeyResultsByQueryType".FromResourceDictionary() +
                                                           " {3} " + "LangKeyAndQueryValue".FromResourceDictionary() +
                                                           " {4} " + "LangKeyTo".FromResourceDictionary() + " {5}\t" +
                                                           CodeConstants.FoundXResults;

        public static string AlreadyExistQuery { get; set; } = "{0}\t {1}\t {2}\t " +
                                                               "LangKeyIndexIesAreAlreadyAddedIn"
                                                                   .FromResourceDictionary() + " {2} " +
                                                               "LangKeySearchQueryIes".FromResourceDictionary() +
                                                               " {3}\t" + CodeConstants.AlreadyExistQuery;

        public static string AlreadyExistQueryCount { get; set; } = "{0}\t {1}\t {2}\t " +
                                                                    "LangKeyAreAlreadyAddedIn"
                                                                        .FromResourceDictionary() + " {2}" +
                                                                    "LangKeySearchQueryIes".FromResourceDictionary() +
                                                                    " {3}\t" + CodeConstants.AlreadyExistQueryCount;

        public static string AccountNeedsVerification { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeyNeedsToVerifiedToPerformNextActivities".FromResourceDictionary() + "\t" +
            CodeConstants.AccountNeedsVerification;

        /// <summary>
        ///     0.Account's SocialNetwork
        ///     1.campaign name/AccountName
        ///     2.ActivityType
        ///     3.Message
        /// </summary>
        public static string CustomMessage { get; set; } = "{0}\t {1}\t {2}\t{3}\t" + CodeConstants.CustomMessage;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        /// </summary>
        public static string ProcessCompleted { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyCompleteProcessTo".FromResourceDictionary() + " {2}\t" +
            CodeConstants.ProcessCompleted;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        /// </summary>
        public static string ProcessStarted { get; set; } = "{0}\t {1}\t {2}\t " +
                                                            "LangKeyStartedProcessTo".FromResourceDictionary() +
                                                            " {2}\t" + CodeConstants.ProcessStarted;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = ActivityType
        /// </summary>
        public static string ProcessStopped { get; set; } = "{0}\t {1}\t {2}\t " +
                                                            "LangKeyStoppedProcessTo".FromResourceDictionary() +
                                                            " {2}\t" + CodeConstants.ProcessStopped;


        /// <summary>
        ///     0 = SocialNetwork
        ///     1 = Content
        /// </summary>
        public static string Deleted { get; set; } = "{0}\t {1}\t {2}\t " +
                                                     "LangKeySuccessfullyDeleted".FromResourceDictionary() + "\t" +
                                                     CodeConstants.Deleted;

        /// <summary>
        ///     0 = SocialNetwork
        ///     1 = Content
        /// </summary>
        public static string Added { get; set; } = "{0}\t {1}\t {2}\t " +
                                                   "LangKeySuccessfullyAdded".FromResourceDictionary() + "\t" +
                                                   CodeConstants.Added;

        /// <summary>
        ///     0 =  SocialNetwork
        ///     1 = Content
        /// </summary>
        public static string Exported { get; set; } = "{0}\t {1}\t {2}\t " +
                                                      "LangKeySuccessfullyExported".FromResourceDictionary() + "\t" +
                                                      CodeConstants.Exported;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string NotAddedAccount { get; set; } =
            "{0}\t {1}\t " + "LangKeyAccounts".FromResourceDictionary() + "\t" +
            "LangKeyHavingIssuesToAdd".FromResourceDictionary() + "\t" + CodeConstants.NotAddedAccount;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string AlreadyAddedAccount { get; set; } =
            "{0}\t {1}\t " + "LangKeyAccounts".FromResourceDictionary() + "\t" +
            "LangKeyAlreadyAdded".FromResourceDictionary() + "\t" + CodeConstants.AlreadyAddedAccount;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string AlreadyUpdatingAccount { get; set; } =
            "{0}\t {1}\t " + "LangKeyAccounts".FromResourceDictionary() + "\t" +
            "LangKeyAlreadyStarted".FromResourceDictionary() + "\t" + CodeConstants.AlreadyUpdatingAccount;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string StopUpdatingAccount { get; set; } = "{0}\t {1}\t " +
                                                                 "LangKeyAccountUpdation".FromResourceDictionary() +
                                                                 "\t" + "LangKeyStoppedForFurtherFriendshipUpdate"
                                                                     .FromResourceDictionary() + "\t" +
                                                                 CodeConstants.StopUpdatingAccount;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string StopAllActivitiesOfAccount { get; set; } = "{0}\t {1}\t " +
                                                                        "LangKeyAccountActivities"
                                                                            .FromResourceDictionary() + "\t" +
                                                                        "LangKeyStoppedAllActivitiesOf"
                                                                            .FromResourceDictionary() + " {1}" + "\t" +
                                                                        CodeConstants.StopAllActivitiesOfAccount;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = campaign name
        /// </summary>
        public static string StartPublishing { get; set; } = "{0}\t {1}\t" + "LangKeyPublish".FromResourceDictionary() +
                                                             "\t" + "LangKeyPublishingStartedWith"
                                                                 .FromResourceDictionary() + " {2}\t" +
                                                             CodeConstants.StartPublishing;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = delay in seconds
        /// </summary>
        public static string DelayBetweenPublishing { get; set; } = "{0}\t {1}\t " +
                                                                    "LangKeyPublish".FromResourceDictionary() + "\t" +
                                                                    "LangKeyNextPostWillStartPublishingIn"
                                                                        .FromResourceDictionary() + " {2} " +
                                                                    "LangKeySeconds".FromResourceDictionary() + "\t" +
                                                                    CodeConstants.DelayBetweenPublishing;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = delay in Minutes
        /// </summary>
        public static string DelayBetweenAccountPublishing { get; set; } = "{0}\t {1}\t " +
                                                                    "LangKeyPublish".FromResourceDictionary() + "\t" +
                                                                    "LangKeyNextAccountPublish"
                                                                        .FromResourceDictionary() + " {2} " +
                                                                    "LangKeyMinutes".FromResourceDictionary() + "\t" +
                                                                    CodeConstants.DelayBetweenPublishing;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = delay in seconds
        /// </summary>
        public static string DelayBetweenMultiPost { get; set; } =
            "{0}\t {1}\t" + "LangKeyPublish".FromResourceDictionary() + "\t" +
            "Next post will start publishing in {2} minutes" + "\t" + CodeConstants.DelayBetweenMultiPost;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        /// </summary>
        public static string PostExpired { get; set; } = "{0}\t {1}\t {2}\t " +
                                                         "LangKeyPostAlreadyExpired".FromResourceDictionary() + "\t" +
                                                         CodeConstants.PostExpired;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Destination type
        ///     3 = Destination Url
        /// </summary>
        public static string PublishingSuccessfully { get; set; } =
            "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" +
            "LangKeyPublishedSuccessfullyOn".FromResourceDictionary() + " {2}-[{3}]\t" +
            CodeConstants.PublishingSuccessfully;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Destination type
        ///     3 = Destination Url
        /// </summary>
        public static string PublishingFailed { get; set; } =
            "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" +
            "LangKeyErrorWhilePublishingOn".FromResourceDictionary() + " {2}-[{3}]\t" + CodeConstants.PublishingFailed;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Destination type
        ///     3 = Destination Url
        /// </summary>
        public static string SharedSuccessfully { get; set; } =
            "{0}\t {1}\t {2}\t " + "LangKeySharedSuccessfullyOn".FromResourceDictionary() + " {2}-[{3}]\t" +
            CodeConstants.SharedSuccessfully;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Destination type
        ///     3 = Destination Url
        /// </summary>
        public static string ShareFailed { get; set; } = "{0}\t {1}\t {2}\t " +
                                                         "LangKeyErrorWhileSharingOn".FromResourceDictionary() +
                                                         " {2}-[{3}]\t" + CodeConstants.ShareFailed;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Post Source
        /// </summary>
        public static string UploadingMedia { get; set; } = "{0}\t {1}\t " + "LangKeyMedia".FromResourceDictionary() +
                                                            "\t" +
                                                            "LangKeyUploadingMediaFile".FromResourceDictionary() +
                                                            " [{2}]\t" + CodeConstants.UploadingMedia;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Post Source
        /// </summary>
        public static string UploadingMediaSuccessful { get; set; } =
            "{0}\t {1}\t " + "LangKeyMedia".FromResourceDictionary() + "\t" +
            "LangKeySuccessfullyUploadedMediaFile".FromResourceDictionary() + " [{2}]\t" +
            CodeConstants.UploadingMediaSuccessful;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Post Source
        /// </summary>
        public static string UploadingMediaFailed { get; set; } =
            "{0}\t {1}\t " + "LangKeyMedia".FromResourceDictionary() + "\t" +
            "LangKeyErrorWhileUploadingMediaFile".FromResourceDictionary() + " [{2}]\t" +
            CodeConstants.UploadingMediaFailed;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Failed reason
        /// </summary>
        public static string UploadingMediaFailedReason { get; set; } =
            "{0}\t {1}\t" + "LangKeyMedia".FromResourceDictionary() + "\t" +
            "LangKeyFailedDueTo".FromResourceDictionary() + " [{2}]\t" + CodeConstants.UploadingMediaFailedReason;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = destination type
        ///     3 = destination Value
        /// </summary>
        public static string NoPost { get; set; } =
            "{0}\t {1}\t " + "LangKeyNoMorePostAvailable".FromResourceDictionary() + " {2}[{3}].";

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = CampaignName
        /// </summary>

        public static string PublishingProcessCompleted { get; set; } =
            "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" +
            "LangKeyPublishingProcessCompleted".FromResourceDictionary() + " -[{2}]\t" +
            CodeConstants.PublishingProcessCompleted;

        public static string ProxyVerificationStarted { get; set; } =
            "{0}\t {1}\t " + "LangKeyProxyVerification".FromResourceDictionary() + "\t" +
            "LangKeyProxyVerificationStarted".FromResourceDictionary() + "\t" + CodeConstants.ProxyVerificationStarted;

        public static string ProxyVerificationCompleted { get; set; } =
            "{0}\t {1}\t " + "LangKeyProxyVerification".FromResourceDictionary() + "\t" +
            "LangKeyProxyVerificationCompleted".FromResourceDictionary() + "\t" +
            CodeConstants.ProxyVerificationCompleted;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Varification type[Phone/Email]
        /// </summary>
        public static string SentVerificationCode { get; set; } = "{0}\t {1}\t " +
                                                                  "LangKeyVerification".FromResourceDictionary() +
                                                                  "\t" +
                                                                  "LangKeyVerificationCodeHasBeenSentToRregistered"
                                                                      .FromResourceDictionary() + " {2}. " +
                                                                  "LangKeyKindlyCheckAndEnterInTheVerificationCodeField"
                                                                      .FromResourceDictionary() + "\t" +
                                                                  CodeConstants.SentVerificationCode;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Varification type[Phone/Email]
        /// </summary>
        public static string FailedToSendVerificationCodeFaild { get; set; } = "{0}\t {1}\t " +
                                                                               "LangKeyVerification"
                                                                                   .FromResourceDictionary() + "\t" +
                                                                               "LangKeyfailedToSendVerificationCodeToRregistered"
                                                                                   .FromResourceDictionary() +
                                                                               " {2}. " + "LangKeyPleaseTryAgain"
                                                                                   .FromResourceDictionary() + "\t" +
                                                                               CodeConstants
                                                                                   .FailedToSendVerificationCodeFaild;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Destination type
        ///     3 = Destination Url
        ///     3 = Error Details
        /// </summary>
        public static string PublishingFailedWithError { get; set; } = "{0}\t {1}\t " +
                                                                       "LangKeyPublish".FromResourceDictionary() +
                                                                       "\t" + "LangKeyErrorWhilePublishingOn"
                                                                           .FromResourceDictionary() +
                                                                       " {2}-[{3}] with error {4}\t" +
                                                                       CodeConstants.PublishingFailedWithError;

        /// <summary>
        ///     0 = Account's SocialNetwork
        ///     1 = Account's Username
        ///     2 = Varification type[Phone/Email]
        /// </summary>
        public static string SentVerificationCodeForAutoVerify { get; set; } = "{0}\t {1}\t " +
                                                                               "LangKeyVerification"
                                                                                   .FromResourceDictionary() + "\t" +
                                                                               "LangKeyVerificationCodeHasBeenSentToRregistered"
                                                                                   .FromResourceDictionary() +
                                                                               " {2}. " + "\t" +
                                                                               CodeConstants.SentVerificationCode;
    }
}