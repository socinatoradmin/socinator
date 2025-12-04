using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace LinkedDominatorCore.LDUtility
{
    public class HtmlAgilityHelper
    {
        public static string MethodGetStringFromId(string response, string id)
        {
            var idPart = string.Empty;
            try
            {
                var htmlDoc = GetDocumentObject;
                htmlDoc.LoadHtml(response);
                idPart = htmlDoc.GetElementbyId(id).OuterHtml;
            }
            catch (Exception)
            {
                //
            }

            return idPart;
        }

        public static string MethodGetInnerStringFromId(string Response, string Id)
        {
            var IdPart = string.Empty;
            try
            {
                var htmlDoc = GetDocumentObject;
                htmlDoc.LoadHtml(Response);
                IdPart = htmlDoc.GetElementbyId(Id)?.InnerHtml;
            }
            catch (Exception)
            {
                //
            }

            return IdPart;
        }

        public static string GetStringInnerTextFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = string.Empty;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                Result = node?.InnerText?.Trim();
            }
            catch
            {
            }

            return Result;
        }

        public static string GetStringInnerTextFromClassName(HtmlNode htmlNode, string ClassName)
        {
            var Result = string.Empty;

            try
            {
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlNode.SelectSingleNode(xpath);
                Result = node.InnerText.Trim();
            }
            catch
            {
            }

            return Result;
        }

        public static string GetStringFromClassName(string Response, string ClassName, HtmlDocument htmlDoc = null,string id="")
        {
            var Result = string.Empty;
            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }
                var xpath = string.IsNullOrEmpty(ClassName)? string.Format("//*[contains(@id,'{0}')]", id) : string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                Result = node.OuterHtml.Trim();
            }catch{}
            return Result;
        }

        public static string GetStringInnerHtmlFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null, HtmlNode htmlNode = null)
        {
            var Result = string.Empty;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                if (htmlNode == null)
                    htmlNode = htmlDoc.DocumentNode;
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlNode.SelectSingleNode(xpath);
                if(node != null)
                    Result = node.InnerHtml.Trim();
            }
            catch
            {
            }

            return Result;
        }

        public static List<string> GetListInnerHtmlFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = GetListStingObject;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                foreach (var singleNode in nodes) Result.Add(singleNode.InnerHtml);
            }
            catch
            {
            }

            return Result;
        }

        public static List<string> GetListHtmlFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = GetListStingObject;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if(nodes != null)
                {
                    foreach (var singleNode in nodes) 
                        Result.Add(singleNode.OuterHtml);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static List<HtmlNode> GetListNodesFromClassName(string Response, string ClassName,
            HtmlDocument htmlDoc = null)
        {
            var Result = GetListHtmlNodeObject;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if(nodes != null)
                    foreach (var singleNode in nodes) 
                        Result.Add(singleNode);
            }
            catch
            {
            }

            return Result;
        }
        public static List<HtmlNode> GetListNodesFromCustomTag(string Response,string tagname, string tagvalue,
            HtmlDocument htmlDoc = null)
        {
            var Result = GetListHtmlNodeObject;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@{0},'{1}')]",tagname, tagvalue);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if (nodes != null)
                    foreach (var singleNode in nodes)
                        Result.Add(singleNode);
            }
            catch
            {
            }

            return Result;
        }
        public static List<HtmlNode> GetListNodesFromAttibute(string response, string attributeType,
            AttributeIdentifierType attributeIdentifierType, HtmlDocument htmlDoc, params string[] containsList)
        {
            var Result = GetListHtmlNodeObject;

            try
            {
                htmlDoc = HtmlDocument(response, htmlDoc);
                var nodeCollection = htmlDoc.DocumentNode.SelectNodes($"//{attributeType}");
                foreach (var node in nodeCollection)
                {
                    var outerHtml = node.OuterHtml;
                    var idOrClassName = "";
                    if (!Utils.IsContains(outerHtml, containsList)
                        || attributeIdentifierType.Equals(AttributeIdentifierType.Id) && string.IsNullOrEmpty(node?.Id))
                        continue;

                    if (attributeIdentifierType.Equals(AttributeIdentifierType.Id))
                        idOrClassName = node.Id;
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.componentkey))
                        idOrClassName = node.GetAttributeValue("componentkey", def: string.Empty);
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.DataViewName))
                        idOrClassName = node.GetAttributeValue("data-view-name", def: string.Empty);
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.ClassName))
                        idOrClassName = Utils.GetBetween(outerHtml, "class=\"", "\"");
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.Xpath))
                        idOrClassName = node.XPath;
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.AriaLabel))
                        idOrClassName = node.Attributes["aria-label"]?.Value;
                    else if (attributeIdentifierType.Equals(AttributeIdentifierType.Href))
                        idOrClassName = node.Attributes["href"]?.Value;
                    if (string.IsNullOrEmpty(idOrClassName))
                        continue;

                    Result.Add(node);
                }
            }
            catch
            {
            }

            return Result;
        }

        private static HtmlDocument HtmlDocument(string response, HtmlDocument htmlDoc)
        {
            if (htmlDoc?.Text == null)
            {
                htmlDoc = GetDocumentObject;
                htmlDoc.LoadHtml(response);
            }

            return htmlDoc;
        }

        public static string GetValueWithAttributeNameFromInnerHtml(string Response, string ClassName,
            string AttributeName, HtmlDocument htmlDoc = null)
        {
            var Result = string.Empty;

            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                var TempResult = node.InnerHtml.Trim();
                var startString = AttributeName + "=\"";
                var endString = "\"";
                Result = Utils.GetBetween(TempResult, startString, endString);
                Result = Result.Trim();
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static List<string> GetListValueWithAttributeNameFromInnerHtml(string Response, string ClassName,
            string AttributeName, HtmlDocument htmlDoc = null)
        {
            var Result = GetListStingObject;
            try
            {
                if (htmlDoc?.Text == null)
                {
                    htmlDoc = GetDocumentObject;
                    htmlDoc.LoadHtml(Response);
                }

                var startString = AttributeName + "=\"";
                var endString = "\"";
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if (nodes != null)
                    foreach (var singleNode in nodes)
                        Result.Add(Utils.GetBetween(singleNode.InnerHtml, startString, endString));
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static List<string> GetListValueWithAttributeNameFromClassName(string Response, string ClassName,
            string AttributeName)
        {
            var Result = GetListStingObject;
            try
            {
                var htmlDoc = GetDocumentObject;
                htmlDoc.LoadHtml(Response);
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var nodes = htmlDoc.DocumentNode.SelectNodes(xpath);
                if (nodes != null)
                    foreach (var singleNode in nodes)
                        Result.Add(singleNode.Attributes[AttributeName].Value);
            }
            catch
            {
                return Result;
            }

            return Result;
        }
        public static List<string> GetListInnerHtmlOrInnerTextOrOuterHtmlFromIdOrClassName(string Response,string Id,bool getInnerText=false,string className = "",bool getOuterHtml=false)
        {
            var result = GetListStingObject;
            var htmlDocument = GetDocumentObject;
            htmlDocument.LoadHtml(Response);
            var xpath = string.IsNullOrEmpty(className) ? string.Format("//*[contains(@id,'{0}')]", Id): string.Format("//*[contains(@class,'{0}')]", className);
            var nodes = htmlDocument.DocumentNode.SelectNodes(xpath);
            if(nodes != null)
            {
                foreach (var node in nodes)
                    result.Add(getInnerText ? node.InnerText?.Trim() : getOuterHtml ? node.OuterHtml : node.InnerHtml);
            }
            return result;
        }
        public static string GetStringTextFromClassName(string Response, string ClassName)
        {
            var Result = string.Empty;

            try
            {
                var htmlDoc = GetDocumentObject;
                htmlDoc.LoadHtml(Response);
                var xpath = string.Format("//*[contains(@class,'{0}')]", ClassName);
                var node = htmlDoc.DocumentNode.SelectSingleNode(xpath);
                if(node != null)
                    Result = node.InnerHtml.Trim();
            }
            catch
            {
            }

            return Result;
        }
        public static List<string> GetListInnerTextFromTagName(string Response,string TagName)
        {
            var list = GetListStingObject;
            try
            {
                var htmlDoc = GetDocumentObject;
                htmlDoc.LoadHtml(Response);
                var Nodes = htmlDoc.DocumentNode.SelectNodes(string.Format("//{0}", TagName));
                foreach (var node in Nodes)
                    list.Add(node.InnerText);
            }
            catch { }
            return list;
        }
        public static HtmlDocument GetDocumentObject => new HtmlDocument();
        public static List<string> GetListStingObject => new List<string>();
        public static List<HtmlNode> GetListHtmlNodeObject=>new List<HtmlNode>();
    }
}