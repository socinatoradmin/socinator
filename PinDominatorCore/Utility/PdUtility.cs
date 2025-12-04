using DominatorHouseCore;
using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WMPLib;

namespace PinDominatorCore.Utility
{
    public class PdUtility
    {
        public void GoInsideListMakeString(ref StringBuilder sb, object obj, string startingObjName)
        {
            var listProperties = obj.GetType().GetProperties();

            foreach (var element in listProperties)
            {
                var startingObjStringName = startingObjName;
                try
                {
                    var elemName = element.PropertyType.Name;
                    if (elemName == "int" || elemName == "Boolean" || elemName == "Int32")
                    {
                        var value = elemName == "Boolean"
                            ? element.GetValue(obj, null).ToString().ToLower()
                            : element.GetValue(obj, null).ToString();
                        if (value != null)
                            sb.AppendLine(GetString(element.Name, value, startingObjStringName, true));
                    }
                    else if (elemName == "String")
                    {
                        var value = element.GetValue(obj, null);
                        if (value != null)
                            sb.AppendLine(GetString(element.Name, value.ToString(), startingObjStringName));
                    }
                    else if (elemName.Contains("List"))
                    {
                        var value = element.GetValue(obj, new object[0]);
                        startingObjStringName = startingObjName + $".{element.Name}";
                        if (value != null)
                            sb.AppendLine(GetStringFromListObj(value, startingObjStringName, 1));
                    }
                    else
                    {
                        var value = element.GetValue(obj, new object[0]);
                        startingObjStringName = startingObjName + $".{element.Name}"; //value.GetType().Name
                        if (value != null)
                            GoInsideListMakeString(ref sb, value, startingObjStringName);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public string GetStringFromListObj(object obj, string startingObjStringName, int numberOfDataFromList = 0)
        {
            try
            {
                var sb = new StringBuilder();
                var value = (IList)obj;
                var iteration = 0;
                if (numberOfDataFromList == 0)
                    numberOfDataFromList = value.Count;

                foreach (var each in value)
                {
                    GoInsideListMakeString(ref sb, each, $"{startingObjStringName}[{iteration++}]");
                    if (iteration >= numberOfDataFromList) break;
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }

        public string GetString(string name, string value, string startingObjStringName, bool isInt = false)
        {
            value = isInt ? value : $"\"{value}\"";
            return $"\n{startingObjStringName}.{name}.Should().Be({value});";
        }

        public static string RemoveSpecialCharacters(string text)
        {
            string[] chars =
            {
                "\\n","%", "?", ",", ".", "/", "!", "@", "#", "$", "^", "&", "*", "'", "\"", ";", "_", "-", "(", ")",
                ":", "|", "[", "]", "♡", "•","\t","\r\n","\n"
            };
            foreach (var character in chars)
                if (text.Contains(character))
                    text = text.Replace(character,character=="\""?"\\\"":character=="\n"?"\\n":character=="\r\n"?"\\n":character=="#"?"%23":"");
            return text;
        }
        public static string RemoveHtmlTags(string pageResponse)
        {
            var removedHtmlTags = string.Empty;
            try
            {
                removedHtmlTags = Regex.Replace(pageResponse, "<.*?>", " ")?.Trim();
                removedHtmlTags = Regex.Replace(removedHtmlTags, "(.*?)>", " ")?.Trim();
                removedHtmlTags = Regex.Replace(removedHtmlTags, "<(.*?)", " ")?.Trim();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return removedHtmlTags;
        }
        public static string AssignNA(string input)
        {
            try
            {
                return string.IsNullOrWhiteSpace(input) ? "N/A" : input?.Trim();
            }
            catch (Exception)
            {
                return "N/A";
            }
        }
        public static string ConvertDecimalToNumber(string number)
        {
            if (string.IsNullOrEmpty(number) ? true : !number.Contains("."))
                return number;
            var Base = number.Contains("k") ? 10 : number.Contains("M") ? 100 : 1000;
            number = number?.Replace("k", "")?.Replace("M", "")?.Trim();
            var decimalLength = number.Split('.').Last().Length.ToString();
            int.TryParse(number?.Replace(".", ""), out int originalNumber);
            int.TryParse(decimalLength, out int DecimalLength);
            return (originalNumber * Math.Pow(Base, DecimalLength)).ToString();
        }
        public static string GetImageResolution(string filePath){try{Bitmap map = new Bitmap(filePath);return $"{map.Width}<:>{map.Height}";}catch (Exception){return "250<:>450";}}
        public static string GetAspectRatioOfAnImage(int Width, int Height) => ((float)Width / Height).ToString();
        public static string ReplaceSpecialCharacter(string InputString) =>
            string.IsNullOrEmpty(InputString) ? InputString : InputString?.Replace("\r\n", "\\n")?.Replace("\t", "\\t")?.Replace("\r", "");

        public static string GetVideoDuration(string mediaPath)
        {
            try
            {
                var player = new WindowsMediaPlayer();
                var clip = player.newMedia(mediaPath);
                var dr = clip.duration;
                return (dr*1000).ToString();
            }
            catch { 
            return string.Empty;
            }
        }
    }
}