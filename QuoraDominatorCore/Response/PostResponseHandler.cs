using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using HtmlAgilityPack;
using QuoraDominatorCore.QdUtility;

namespace QuoraDominatorCore.Response
{
    public class PostResponseHandler : QuoraResponseHandler
    {
        public Dictionary<int, string> PostUrl = new Dictionary<int, string>(); //1

        public PostResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (RespJ == null)
                {
                    HtmlNode[] posts = null;
                    try
                    {
                        posts = HtmlDocument.DocumentNode.SelectNodes("//a[@class='timestamp board_timestamp']")
                            .ToArray();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        //posts = HtmlDocument.DocumentNode.SelectNodes("//a[@class='board_timestamp timestamp']").ToArray();
                    }

                    var biid = Regex.Split(response.Response, "biid");
                    foreach (var bii in biid)
                        try
                        {
                            var bid = int.Parse(Utilities.GetBetween(bii, "\": ", ",").Replace("}", ""));
                            PostUrl.Add(bid, null);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    try
                    {
                        for (var i = 0; i < posts.Length; i++)
                        {
                            var url = posts[i].Attributes["href"].Value;
                            if (url.Contains("profile"))
                                url = $"{QdConstants.HomePageUrl}" + url;
                            PostUrl[PostUrl.ElementAt(i).Key] = url;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                else
                {
                    var objHtmlDocument = new HtmlDocument();
                    objHtmlDocument.LoadHtml(response.Response);
                    HtmlDocument = objHtmlDocument;
                    HtmlNode[] posts;
                    try
                    {
                        posts = HtmlDocument.DocumentNode.SelectNodes("//a[@class='timestamp board_timestamp']")
                            .ToArray();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                        posts = HtmlDocument.DocumentNode.SelectNodes("//a[@class='board_timestamp timestamp']")
                            .ToArray();
                    }

                    var biid = Regex.Split(response.Response, "biid");
                    foreach (var bii in biid)
                        try
                        {
                            var bid = int.Parse(Utilities.GetBetween(bii, "\": ", ",").Replace("}", ""));
                            PostUrl.Add(bid, null);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    try
                    {
                        for (var i = 0; i < posts.Length; i++)
                        {
                            var url = posts[i].Attributes["href"].Value;
                            if (url.Contains("profile"))
                                url = QdConstants.HomePageUrl + url;
                            PostUrl[PostUrl.ElementAt(i).Key] = url;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}