using System;
using System.Linq;
using DominatorHouseCore.Utility;
using System.IO;
using System.Security.Cryptography;
using DominatorHouseCore;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.GDUtility
{
    public static class GdUtilities
    {
        private static readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        //public static List<string> FileBrowse(bool isSelectMultiple, string filter)
        //{
        //    var files = new List<string>();

        //    var openFileDialog = new Microsoft.Win32.OpenFileDialog
        //    {
        //        Multiselect = isSelectMultiple,
        //        Filter = filter
        //    };

        //    var openFileDialogResult = openFileDialog.ShowDialog();

        //    if (openFileDialogResult != true) return new List<string>();

        //    foreach (var fileName in openFileDialog.FileNames)
        //    {
        //        try
        //        {
        //            var extension = Path.GetExtension(fileName);

        //            if (extension != null && (filter.Contains(extension))) continue;

        //            files.Add(fileName);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.StackTrace);
        //        }
        //    }

        //    return files;
        //}

        //public static string ConvertToYesNo(this string valueInBool)
        //{
        //    return (valueInBool.ToLower() == "true") ? "Yes" : "No";
        //}

        public static int GetRandomNumber(int noOfLength)
        {
            return Convert.ToInt32(new string(Enumerable.Repeat("1234567890", noOfLength)
                .Select(s => s[RandomUtilties.ObjRandom.Next(s.Length)]).ToArray()));
        }
        public static string GetChecksum(string file)
        {
            using (FileStream stream = File.OpenRead(file))
            {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }
        /// <summary>
        ///  Removes special characters from a text string
        /// </summary>
        /// <param name="text">Text from where special characters will remove </param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string text)
        {
            string[] chars = new[] { "\\n", "?", ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "-", "(", ")", ":", "|", "[", "]", "♡", "•" };
            foreach (var character in chars)
            {
                if (text.Contains(character))
                {
                    text = text.Replace(character, "");
                }
            }

            return text;
        }
        public static bool IsContains(string pageSource, params string[] containsList)
        {
            foreach (var contain in containsList)
                if (pageSource.Contains(contain))
                    return true;

            return false;
        }
        public static int ConvertDoubleAndInt(string input)
        {
            var doubleResult = Convert.ToDouble(input);
            return Convert.ToInt32(doubleResult);

        }

        public static string GetRowClientTime()
        {
            string Rawclienttime = ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            string initialtime = Rawclienttime.Substring(0, Rawclienttime.Length - 3);
            string endtime = Rawclienttime.Substring(Rawclienttime.Length - 3);
            Rawclienttime = initialtime + "." + endtime;
            return Rawclienttime;
        }

        public static string CreateJson()
        {
            //#region Export Follower in text file in json format just for checking

            //string docPath = @"C:\Users\GLB-123\Downloads";
            //string data = string.Empty;
            //int count = 0;
            //data = "{\"Followers\" :[";
            //foreach (InstagramUser instagramUser in lstInstagramUser)
            //{
            //    if (count == 0)
            //    {
            //        data = data + JsonConvert.SerializeObject(instagramUser);
            //        count++;
            //    }
            //    else
            //    {
            //        data = data + "," + JsonConvert.SerializeObject(instagramUser);
            //    }
            //}
            //data = data + "]}";
            //using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.IO.Path.Combine(docPath, "Followers.txt")))
            //{
            //    outputFile.WriteLine(data);
            //}
            //#endregion
            return "";
        }
        public static string CreatePostUrl(string QueryValue)
        {
            var postUrl = Constants.gdHomeUrl;
            try
            {
                var ActualValue = Regex.Replace(QueryValue, "/\\?.*", "");
                var postId = ActualValue.Split('/').LastOrDefault(x=> x != string.Empty);
                return $"{postUrl}/p/{postId}/";
            }catch (Exception ex)
            {
                ex.DebugLog();
            }
            return postUrl;
        }
        public static string GetOwnLikedPostJToken(string Json)
        {
            JToken token = null;
            try
            {
                var jObject = handler.ParseJsonToJObject(Json);
                var JArray = handler.GetJArrayElement(handler.GetJTokenValue(jObject, "tree", "bk.components.screen.Wrapper", "content", "bk.components.Flexbox",
                                                                "children", 2, "bk.components.Flexbox", "children", 0,
                                                                "bk.components.Flexbox", "children", 0,
                                                                "bk.components.Collection", "children"));
                if(JArray != null && JArray.HasValues)
                {
                    foreach(var item  in JArray)
                    {
                        token = handler.GetJTokenOfJToken(item, "bk.components.Flexbox",
                                                                "children");
                        if (token != null && token.HasValues)
                            break;
                    }
                }
            }catch (Exception) { }
            return token is null ?Json:token.ToString();
        }
    }
}
