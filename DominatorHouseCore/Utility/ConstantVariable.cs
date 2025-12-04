#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using Newtonsoft.Json;

#endregion

namespace DominatorHouseCore.Utility
{
    public static class ConstantVariable
    {
        public static string UseragentCommonFormat { get; } =
            "Instagram {0} Android ({1}/{2}; {3}; {4}; {5}; {6}; {7}; {8}; {9})";

        public static string TikTokUserAgentCommonFormat { get; } =
            "{0}/{1} (Linux; U; Android {2}; {3}; {4}; Build/{5}; Cronet/58.0.2991.0)";

        public static string UserAgentDomain = "com.zhiliaoapp.musically";
        public static bool UsePuppeteer => false;
        public static string FFMPegPath { get; set; }
        public static string FFMPegExecutable { get; set; } = "https://storage.googleapis.com/empmonitor-cctv-dev/CCTV-P/ffmpeg.exe";
        public static string DependencyFile { get; set; } = "https://www.dropbox.com/scl/fi/ausmqdhkfs940fy9bdn2t/Dependency.zip?rlkey=59h5nskfjihrd1r2vvetf5i20&dl=1";
        public static string TikTokManifestVersion { get; } = "2018110931";

        public static string UseragentLocale { get; } = "en_US;";

        // public static string IgVersion { get; } = "40.33.0";
        public static string IgVersion { get; } =
            "283.0.0.20.105";//"253.0.0.23.114";//"144.0.0.25.119"; //"117.0.0.28.123";//"107.0.0.27.121 ";//"94.0.0.22.116";

        public static string ApiUrl => $"{(object)InstagramBaseUrl}api/v1/";

        public static string InstagramBaseUrl { get; } = "https://i.instagram.com/";

        public static string ApplicationName { get; } = /* "Socinator";*/ "LangKeySocinator".FromResourceDictionary();
        public static string AssemblyName => $"{ApplicationName}1.0";
        public static string BitlyApiKey { get; set; } = string.Empty;

        public static string BitlyLogin { get; set; } = string.Empty;
        public static string Revision { get; set; }


        public static string GetPlatformBaseDirectory()
        {
            var basePath =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{AssemblyName}";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }
        public static string GetCefDependency()
        {
            var basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\CefDependency";
            DirectoryUtilities.CreateDirectory(basePath);
            return basePath;
        }
        public static string GetPlatformTodayBackupDirectory()
        {
            var basePath =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{AssemblyName}Backup";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }
        public static string GetFFMPegPath()
        {
            var dir = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\FFMpeg";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetVisualPath()
        {
            var dir = $"{GetCefDependency()}\\Visual";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetChromeDir()
        {
            var dir = $"{GetPlatformBaseDirectory()}\\Chrome";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetPlatformLogDirectory()
        {
            var basePath =
                $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{AssemblyName}\\logs";

            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }

        public static string GetDesktopSocNetDirectory(SocialNetworks net = SocialNetworks.Social)
        {
            return CreateDirIfNot(
                $"{Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)}\\{AssemblyName}{(net == SocialNetworks.Social ? "" : "\\" + net)}");
        }
        public static string GetPopupDetailsFile()
        {
            return $"{GetOtherDir()}\\PopUpConfig.txt";
        }
        public static SubscribtionPopUpModel GetPopupdetails()
        {
            try
            {
                var text = File.ReadAllText(GetPopupDetailsFile());
                return string.IsNullOrEmpty(text) ? null : JsonConvert.DeserializeObject<SubscribtionPopUpModel>(text);
            }
            catch { return null; }
        }
        public static bool SavePopupInfo(SubscribtionPopUpModel model)
        {
            try
            {
                File.WriteAllText(GetPopupDetailsFile(),JsonConvert.SerializeObject(model));
                return true;
            }
            catch { return false; }
        }
        public static string GetDebugResponseFile(SocialNetworks net, string username)
        => CreateDirIfNot($"{GetDesktopSocNetDirectory(net)}\\{username}\\DebugResponses") + $"\\{DateTime.Now.Date.GetCurrentEpochTime()}.txt";

        public static string CreateDirIfNot(string basePath)
        {
            if (!Directory.Exists(basePath))
                DirectoryUtilities.CreateDirectory(basePath);

            return basePath;
        }

        public static string GetConfigurationDir()
        {
            var dir = $"{GetPlatformBaseDirectory()}\\Configurations";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetConfigurationDir(SocialNetworks network)
        {
            var dir = $"{GetPlatformBaseDirectory()}\\Configurations\\{network}";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string CreateCampaign()
        {
            return "LangKey_CreateCampaign".FromResourceDictionary();
        }

        public static string UpdateCampaign()
        {
            return "LangKey_UpdateCampaign".FromResourceDictionary();
        }

        public static string NoAccountSelected()
        {
            return "LangKeyNoAccountSelected".FromResourceDictionary();
        }

        public static string GetIndexAccountDir()
        {
            var dir = GetPlatformBaseDirectory() + @"\Index\AC";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetIndexAccountFile()
        {
            return GetIndexAccountDir() + @"\AccountDetails.bin";
        }


        public static string GetIndexCampaignDir()
        {
            var dir = GetPlatformBaseDirectory() + @"\Index\CP";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetIndexCampaignFile()
        {
            return GetIndexCampaignDir() + @"\CampaignDetails.bin";
        }

        public static string GetTemplatesFile()
        {
            return GetConfigurationDir() + "\\Template.bin";
        }

        public static string UnGrouped()
        {
            return "LangKeyUngrouped".FromResourceDictionary();
        }

        public static string DateasFileName { get; set; } = DateTime.Now.ToString("ddMMyyyyHmmss");

        public static string GetDate()
        {
            return DateTime.Now.ToString("ddMMyyyy");
        }

        public static string GetDateTime()
        {
            return DateTime.Now.ToString("ddMMyyyyHmmss");
        }

        public static string GetHourDateTime()
        {
            return DateTime.Now.ToString("Hmmss.ff");
        }

        public static string GoogleLink { get; set; } = "https://www.google.com";

        public static string GetOtherDir()
        {
            var dir = $"{GetPlatformBaseDirectory()}\\Other";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetCapSolverExtension()
        {
            var dir = $"{GetOtherDir()}\\CapSolver";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetDebugActivityDirectory()
        {
            var dir = $"{GetPlatformBaseDirectory()}\\DebugActivities";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string GetProcessedDestinationDir()
        {
            var dir = $"{GetPlatformBaseDirectory()}\\Other\\PublisherProcessedDestination";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }


        public static string GetOtherProxyFile()
        {
            return GetOtherDir() + @"\Proxy.bin";
        }

        public static string GetOtherPostsFile()
        {
            return GetOtherDir() + @"\Posts.bin";
        }

        public static string GetOtherConfigFile()
        {
            return GetOtherDir() + @"\Config.bin";
        }

        public static string GetPublisherFile()
        {
            return GetOtherDir() + @"\PublisherAccountDetails.bin";
        }

        public static string GetPublisherDestinationsFile()
        {
            return GetOtherDir() + @"\PublisherManageDestinations.bin";
        }


        public static string GetPublisherCreatePostlistFolder()
        {
            return GetOtherDir() + @"\PostsList\";
        }

        public static string GetPublisherCreateDestinationsFolder()
        {
            return GetOtherDir() + @"\DestinationList\";
        }

        public static string GetPublisherPostlistSettingsFile()
        {
            return GetOtherDir() + @"\PostlistSettings.bin";
        }

        public static string GetThemesFile()
        {
            return GetOtherDir() + @"\Themes.txt";
        }

        public static string GetOtherTemp()
        {
            return GetOtherDir() + @"\temp.bin";
        }

        public static string GetChatDir()
        {
            var dir = $"{GetPlatformBaseDirectory()}\\LiveChat";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetDownloadedMediaFolderPath =>
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static string MyAppFolderPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static bool IsNewUI => "LangKeyNewUI".FromResourceDictionary()!=null && "LangKeyNewUI".FromResourceDictionary()=="Socinator New UI";
        public static string GetNotFoundImage()
        {
            return GetOtherDir() + @"\NotFoundImage.png";
        }
        public static string GetIconExtension() => "Icon.png";
        public static string GetSocinatorIcon()
        {
            return GetOtherDir() + @"\" + $"{"LangKeySocinator".FromResourceDictionary()}{GetIconExtension()}";
        }
        public static string GetOtherEmailNotificationFile()
        {
            return GetOtherDir() + @"\EmailNotification.bin";
        }

        public static string GetOtherEmbeddedBrowserSettingsFile()
        {
            return GetOtherDir() + @"\EmbeddedBrowserSettings.bin";
        }

        public static string GetOtherSoftwareSettingsFile()
        {
            return GetOtherDir() + @"\SoftwareSettings.bin";
        }

        public static string GetOtherFacebookSettingsFile()
        {
            return GetOtherDir() + @"\Facebook.bin";
        }

        public static string GetOtherInstagramSettingsFile()
        {
            return GetOtherDir() + @"\Instagram.bin";
        }

        public static string GetConfigurationKey()
        {
            return $"{GetConfigurationDir()}\\{ApplicationName}Key.bin";
        }

        public static string GetURLShortnerServicesFile()
        {
            return GetOtherDir() + @"\URLShortnerServices.bin";
        }

        public static string GetCaptchaServicesFile()
        {
            return GetOtherDir() + @"\CaptchaServices.bin";
        }

        public static string GetImageCaptchaServicesFile()
        {
            return GetOtherDir() + @"\ImageCaptchaServices.bin";
        }
        public static string GetFunCaptchaServicesFile()
        {
            return GetOtherDir() + @"\FunCaptchaService.bin";
        }
        public static string GetOtherProxyManagerSettingsFile()
        {
            return GetOtherDir() + @"\ProxyManagerSettings.bin";
        }

        public static string GetOtherCustomizedAutoActivitySetFile()
        {
            return GetOtherDir() + @"\CustomAutoActSetSettings.bin";
        }

        public static string SaveAction { get; set; } = "Save";

        public static string UpdateAction { get; set; } = "Update";

        public static string GetFavoriteTimeFile()
        {
            return GetOtherDir() + @"\FavoriteTime.bin";
        }

        public static string GetOtherQuoraSettingsFile()
        {
            return GetOtherDir() + @"\Quora.bin";
        }

        public static string GetOtherLinkedInSettingsFile()
        {
            return GetOtherDir() + @"\LinkedIn.bin";
        }
        public static string GetOtherRedditSettingsFile()
        {
            return GetOtherDir() + @"\Reddit.bin";
        }
        #region Publisher

        public static string
            FineStatusSync = "LangKeyAlreadyUpToDate".FromResourceDictionary() /*"Already up to date"*/;

        public static string NeedUpdateStatusSync = "Click to Sync";

        public static string NotAvailableAccountSync = "Account not available";

        public static string DraftPostList { get; set; } = "Draft";

        public static string PendingPostList { get; set; } = "Pending";

        public static string PublishedPostList { get; set; } = "Published";

        public static string GetPublisherOtherConfigDir()
        {
            var dir = $"{GetOtherDir()}\\PublisherOtherConfig";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetPublisherOtherConfigFile(SocialNetworks networks)
        {
            return GetPublisherOtherConfigDir() + $"\\{networks}.bin";
        }

        public static string GetPublisherCampaignFile()
        {
            return GetOtherDir() + "\\PublisherCampaign.bin";
        }

        public static string GetPublisherPostFetchFile => GetOtherDir() + "\\PublisherPostFetcherDetails.bin";

        public static string GetMacroDetails => GetOtherDir() + $"\\{ApplicationName}Macros.bin";

        public static string GetPublishedSuccessDetails => GetOtherDir() + "\\PublishedSuccessDetails.bin";

        public static string GetDeletePublisherPostModel => GetOtherDir() + "\\PublisherDeletionPosts.bin";

        public static string Yes { get; set; } = "Yes";

        public static string No { get; set; } = "No";

        public static string NoError { get; set; } = "No Error!";

        public static DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public static string DeletedDateText()
        {
            return $"Post has been deleted on {GetCurrentDateTime()}!";
        }

        public static string NotPublished { get; set; } = "Not Published Yet";

        public static string GetOtherPinterestSettingsFile()
        {
            return GetOtherDir() + @"\Pinterest.bin";
        }

        public static string GetOtherTumblrSettingsFile()
        {
            return GetOtherDir() + @"\Tumblr.bin";
        }

        public static string GetOtherTwitterSettingsFile()
        {
            return GetOtherDir() + @"\Twitter.bin";
        }

        public static string GetOtherYoutubeSettingsFile()
        {
            return GetOtherDir() + @"\YouTube.bin";
        }

        #endregion


        public static string GetAccountDb = "AccountDb";

        public static string GetCampaignDb = "CampaignDb";

        public static IEnumerable<SocinatorIntellisenseModel> FdMacros = new List<SocinatorIntellisenseModel>
        {
            new SocinatorIntellisenseModel {Key = "{AllFriends}", Value = "{AllFriends}"},
            new SocinatorIntellisenseModel {Key = "{Random:1-5}", Value = "{Random:1-5}"},
            new SocinatorIntellisenseModel {Key = "{Random:5-10}", Value = "{Random:5-10}"},
            new SocinatorIntellisenseModel {Key = "{Random:15-20}", Value = "{Random:15-20}"},
            new SocinatorIntellisenseModel {Key = "{Random:10-15}", Value = "{Random:10-15}"},
            new SocinatorIntellisenseModel {Key = "{Random:20+}", Value = "{Random:20+}"}
        };

        public static string Separator = "<:>";

        public static string VideoToImageConvertFileName { get; set; } = $"_{AssemblyName.ToUpper()}IMAGE.jpg";

        public static List<string> SupportedVideoFormat = new List<string>
        {
            "3g2", "3gp", "3gpp", "asf", "avi", "dat", "divx", "dv", "f4v", "flv", "m2ts", "m4v", "mkv", "mod", "mov",
            "mp4", "mpe", "mpeg", "mpeg4", "mpg", "mts", "nsv", "ogm", "ogv", "qt", "tod", "ts", "vob", "wmv"
        };

        public static string VideoToImageConvertPngFileName { get; set; } = $"_{AssemblyName.ToUpper()}IMAGE.png";

        public static string ProcessingInput { get; set; } =
            "https://socinator.com/amember/softsale/api/check-activation?key={0}&request[hardware-id]={1}";

        public static string ProcessingDebugType { get; set; } =
            "https://dominatorhouse.com/amember/softsale/api/check-activation?key={0}&request[hardware-id]={1}";

        public static string FindExemptions { get; set; } =
            "https://socinator.com/amember/softsale/api/check-license?key={0}";

        public static string ExemptionInnerException { get; set; }
            = "https://socinator.com/amember/api/invoices/{0}?_key={1}";

        public static string LogExemptions { get; set; }
            = "https://socinator.com/amember/softsale/api/activate?key={0}&request[hardware-id]={1}";

        public static string DebugLogExemptions { get; set; }
            = "https://dominatorhouse.com/amember/softsale/api/activate?key={0}&request[hardware-id]={1}";

        public static string DebugPower =
            "https://powerof7.io/amember/softsale/api/check-activation?key={0}&request[hardware-id]={1}";

        public static string DebugPowerLogExemptions { get; set; }
            = "https://powerof7.io/amember/softsale/api/activate?key={0}&request[hardware-id]={1}";

        public static string MarketingSoftware { get; set; } = "Marketing Software";
        public static string ContactSupportLink { get; set; } = "https://socinator.com/contact-us/";

        public static bool IsToasterNotificationNeed { get; set; } = true;


        public static string PageOrBoard { get; set; } = "PageOrBoard";

        public static string Group { get; set; } = "Group";

        public static string OwnWall { get; set; } = "OwnWall";
        public static string UpdatedVersionIP { get; set; } = "209.250.252.53";

        public static string UpdateVersionFilePath { get; set; } = "SocialInstaller/Socinator.txt";

        public static string UpdateInstallerFilePath { get; set; } = "SocialInstaller/Socinator.msi";

        public static string UpdateVersionLink { get; set; } =
            "http://{0}/{1}";

        public static string GetFacebookDetailsConfigFile()
        {
            return GetOtherDir() + @"\FacebokDetails\FacebookEntity.bin";
        }

        public static string GetDownloadMediaPath()
        {
            var dir = Path.Combine(GetPlatformBaseDirectory(),"Downloaded Medias");
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string PageInviterNote => "LangKeyPageInviterNote".FromResourceDictionary();

        public static string SocialAccountManagerVideoLink =>
            "https://www.youtube.com/playlist?list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE";

        public static string FbAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=R-ZJTZ1_SJg&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=2&t=0s";

        public static string IgAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=oaDKQ1bg1sk&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=2";

        public static string LdAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=qBlgzMm756s&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=3";

        public static string QdAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=g0oHEjXUr2A&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=4";

        public static string RdAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=nwXqBOC0Hq0&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=5";

        public static string TmblrAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=9s3i-2U-Nas&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=6";

        public static string TdAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=vcvos6uAhiI&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=7";

        public static string PdAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=RK2nzfJRudc&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=8";

        public static string YtAccountManagerVideoLink =>
            "https://www.youtube.com/watch?v=SWj2OdU_7Ts&list=PL60e8mIWfxoaY8utTkKYXCL6ULzlb3TeE&index=9";

        public static string MediaTempFolder => $@"{GetDownloadedMediaFolderPath}\{AssemblyName}\Temp";
    }

    public static class FileDirPath
    {
        public static string GetChatDir(SocialNetworks network)
        {
            var dir = $"{ConstantVariable.GetPlatformBaseDirectory()}\\LiveChat\\{network}";
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }

        public static string GetChatDetailFile(SocialNetworks network)
        {
            return GetChatDir(network) + "\\Chat.bin";
        }

        public static string GetFriendDetailFile(SocialNetworks network)
        {
            return GetChatDir(network) + "\\Friend.bin";
        }
    }
}