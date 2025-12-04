using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Response
{
    public class ActivityResponseHandler : QuoraResponseHandler
    {
        public ActivityResponseHandler(IResponseParameter response) : base(response)
        {
            if (Regex.Split(response.Response, "EventHeader pass_color_to_child_links many_faces").Length == 1)
                return;
            var activities = HtmlDocument.DocumentNode
                .SelectNodes("//div[@class='EventHeader pass_color_to_child_links many_faces']").ToArray();
            foreach (var activity in activities)
            {
                var s = activity.InnerHtml;
                Utilities.GetBetween(s, "</span></span></span>", "<span class");
                //string url = "https://www.quora.com"+ Utilities.GetBetween(s, "<span class='timestamp'><a href='", "'");
                // var replace = Regex.Unescape(Regex.Replace(url, "\\\\([^u])", "\\\\$1")).Replace("\\", "");
            }
        }
    }
}