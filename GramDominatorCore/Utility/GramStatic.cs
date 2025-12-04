using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDUtility;
using NReco.VideoConverter;
using NReco.VideoInfo;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GramDominatorCore.Utility
{
    public static class GramStatic
    {
        private static readonly Random random = new Random();
        public static bool GetWebFollowerFollowing => true;
        public static bool IsBrowser => false;
        public static string AcceptEncoding => "gzip,deflate";
        public static string MediaSizeLimitReached => "The file that you have selected is too large. The maximum size is 25 MB";
        public static string MessageRestricted => "Not everyone can message this account.";
        public static string GetCloseFriendListUrl => "https://www.instagram.com/accounts/close_friends/";
        public static string GetHastagUrl(string HastagName) => $"https://www.instagram.com/explore/tags/{HastagName}/";
        /// <summary>
        /// Get a deleting query from the list of QueryContent Just by comparing the another QueryContent with any of the list
        /// </summary>
        /// <param name="QueryList">The list of QueryContent</param>
        /// <param name="queryToDelete">the another QueryContent to compare</param>
        /// <returns></returns>
        public static QueryContent GetDeletingQuery(this ObservableCollection<QueryContent> QueryList, QueryContent queryToDelete)
        => QueryList.FirstOrDefault(x => x.Content.QueryType == queryToDelete.Content.QueryType && x.Content.QueryValue == queryToDelete.Content.QueryValue);

        public static System.Drawing.Rectangle ScreenResolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

        public static MediaInfo GetMediaInfo(string mediaPath)
        {
            var ffProbe = new FFProbe();
            ffProbe.ToolPath = ConstantVariable.GetOtherDir();
            return ffProbe.GetMediaInfo(mediaPath);
        }

        public static string GetVideoThumb(string mediaPath, string convertedMediaFilePath)
        {
            FFMpegConverter ffMpegConverter = new FFMpegConverter();
            ffMpegConverter.FFMpegToolPath = ConstantVariable.GetOtherDir();
            var thumbnailFilePath = $@"{Path.GetDirectoryName(mediaPath)}\{Path.GetFileNameWithoutExtension(mediaPath)}.jpg";
            ffMpegConverter.GetVideoThumbnail(
                    (!File.Exists(convertedMediaFilePath) ? mediaPath : convertedMediaFilePath), thumbnailFilePath, 3);
            return thumbnailFilePath;
        }
        public static string GetStoryDownloadPath()
        {
            var dir = Path.Combine(ConstantVariable.GetOtherDir(), "InstaStory");
            DirectoryUtilities.CreateDirectory(dir);
            return dir;
        }
        public static string SanitizeFileName(string input, char replacement = '_')
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(input.Length);

            foreach (var ch in input)
            {
                builder.Append(Array.IndexOf(invalidChars, ch) >= 0 ? replacement : ch);
            }
            return builder.ToString();
        }
        public static string CreateJazoest(string PhoneID = "")
        {
            var ID = string.IsNullOrEmpty(PhoneID) ? Guid.NewGuid().ToString() : PhoneID;
            byte[] buf = Encoding.ASCII.GetBytes(ID);
            int sum = 0;
            for (int i = 0; i < buf.Length; i++)
            {
                sum += buf[i];
            }
            return $"2{sum}";
        }
        public static string GetCodeFromUrlString(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            var tempCode = url;
            if (tempCode.Contains(".instagram.com"))
            {
                var regex = new Regex(@"(?<=/p/|/reels/|/reel/)[\w\-]+");
                var match = regex.Match(tempCode);
                tempCode = match.Success ? match.Value : tempCode;
            }
            return tempCode;
        }
        public static string GetCodeFromIDOrUrl(string IDOrUrl = "")
        {
            if (string.IsNullOrEmpty(IDOrUrl))
                return string.Empty;
            var tempCode = GetCodeFromUrlString(IDOrUrl);
            return (tempCode.GetCodeFromUrl() ?? tempCode).GetIdFromCode();
        }
        public static string GetUrl(string ID, bool User = true, bool Post = false, bool Reel = false)
        {
            return User ?
                $"https://www.instagram.com/{ID}/" :
                Post ?
                $"https://www.instagram.com/p/{ID}/" :
                Reel ?
                $"https://www.instagram.com/reel/{ID}/" :
                $"https://www.instagram.com/tv/{ID}/";
        }
        public static string GetValidBase64(string base64String)
        {
            int missingPadding = base64String.Length % 4;
            if (missingPadding > 0)
            {
                base64String += new string('=', 4 - missingPadding);
            }
            return base64String;
        }

        public static string GetMediaContext()
        {
            var timestampMicroseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000;
            var random = new Random();
            var random9Digit = random.Next(100000000, 1000000000);
            return $"{timestampMicroseconds}_0_{random9Digit}";
        }
        public static string InstagramAjax()
        {
            var random = new Random();
            return (random.Next(100000000, 999999999) * 10L + random.Next(0, 10)).ToString();
        }
        public static string GetWebSessionID()
        {
            return $"{GenerateRandomString(6)}:{GenerateRandomString(6)}:{GenerateRandomString(6)}";
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "a0b1c2d3e4f5g6h7i8j9k0l1m2n3o4p5q6r7s8t9u0v1w2x3y4z56789";
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(chars[random.Next(chars.Length)]);
            return sb.ToString();
        }
    }
}

