using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Response
{
    public class BlogResponseHandler : QuoraResponseHandler
    {
        public BlogResponseHandler(IResponseParameter response) : base(response)
        {
            if (RespJ == null)
            {
                if (Regex.Split(response.Response, "BoardNameLink").Length == 1)
                    return;
                var nodes = HtmlDocument.DocumentNode.SelectNodes("//a[@class='BoardNameLink board_name']").ToArray();
                foreach (var item in nodes)
                {
                    var s = item.OuterHtml;
                    var url = Utilities.GetBetween(s, "href='", "'");
                    if (!url.Contains("http"))
                    {
                    }
                }
            }
        }
    }
}