using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.QdLibrary
{
    public class BasePostData
    {
        public BasePostData(string response)
        {
            try
            {
                try
                {
                    var threadIdRaw = Regex.Split(response, "threadId");
                    foreach(var eachId in threadIdRaw)
                    {
                        var eachIdUrl = Utilities.GetBetween(eachId, "\\\"url\\\":\\\"", "\"");
                        if (eachIdUrl.Contains("messages")) ThreadIds.Add(eachIdUrl);
                    }
                } catch(Exception e) { }
                //string qd = Utilities.GetBetween(response, "type=\"text/javascript\">require.installPageProperties(", ", {\"ad_unit_field_length_limits\"");
                //JObject jsonObject = JObject.Parse(qd);
                //revision = jsonObject["revision"].ToString();
                ParentDomId = Utilities.GetBetween(response, "<div class='table_cell_wrapper'><span id='", "'");
                ForceCid = Utilities.GetBetween(response, "PagedList\", ", ",");
                PageLoadUid = Utilities.GetBetween(response, "{\"viewer\":", ",");
                ParentCid = Utilities.GetBetween(response, "layout_3col_center results_list' id='__w2_",
                    "_results_list");
                Hashes = Utilities.GetBetween(response, "hashes\": ", "]") + "]";
                Hmac = Utilities.GetBetween(response, "serialized_component\": \"#[\\\"", "\\");
                SerializedComponent = Utilities.GetBetween(response, "serialized_component\": \"#", "]") + "]";
                // string lastTwoChar = ParentDomId.Substring(ParentDomId.Length - 2);
                //int lastTwoCharInt = 0;
                //int.TryParse(lastTwoChar, out lastTwoCharInt);
                //ParentCid = ParentDomId.Replace(lastTwoChar, (lastTwoCharInt+1).ToString());
                Revision = Utilities.GetBetween(response, "revision\": \"", "\"");
                FormKey = Utilities.GetBetween(response, "formkey\": \"", "\"");
                WindowId = Utilities.GetBetween(response, "windowId\": \"", "\"");
                PostKey = Utilities.GetBetween(response, "postkey\": \"", "\"");
                ReferringController = Utilities.GetBetween(response, "controller\": \"", "\"");
                ReferringAction = Utilities.GetBetween(response, "action\": \"", "\"");
                PageSourceDataRequired = Utilities.GetBetween(response, "start({", "})");
                ArrayRequiredPostData = Regex.Split(PageSourceDataRequired, ",");
                Guid = System.Guid.NewGuid().ToString();
                PagedList = Utilities.GetBetween(response, "PagedList\", \"", "\"");
                PagedListArray = Regex.Split(response, "\"" + PagedList + "\"");
                NotifsNavItemBase = Utilities.GetBetween(response, "\"NotifsNavItemBase\", \"", "\"");
                NotifsNavItemBaseArray = Regex.Split(response, "\"" + NotifsNavItemBase + "\"");
                ThreadId = Utilities.GetBetween(response, "threadId\": ", ",");
                Uid = Utilities.GetBetween(response, "uid\": ", "}");
                TargetUid = Utilities.GetBetween(response, "target_uid\": ", ",");
                if (ArrayRequiredPostData.Length > 1)
                {
                    Channel = ArrayRequiredPostData[1].Replace("\"", "").Trim();
                    Chan = ArrayRequiredPostData[3].Trim().Replace("\"", "");
                    Hash = ArrayRequiredPostData[2].Replace("\"", "").Trim();
                }
                VconJson = Utilities.GetBetween(PagedListArray[PagedListArray.Length - 2], "\"", "\"");
                Broadcast_Id = Utilities.GetBetween(response, "\"channel\": \"", "\"");
                ParentDomain =
                    Utilities.GetBetween(NotifsNavItemBaseArray[NotifsNavItemBaseArray.Length - 3], "\"", "\"");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region properties 

        public string Channel { get; set; } = string.Empty;
        public string Hmac { get; set; } = string.Empty;
        public string Broadcast_Id { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public string MinSeq { get; set; } = "0";
        public string Revision { get; set; } = string.Empty;
        public string FormKey { get; set; } = string.Empty;
        public string WindowId { get; set; } = string.Empty;
        public string PagedList { get; set; } = string.Empty;
        public string PostKey { get; set; } = string.Empty;
        public string PageSourceDataRequired { get; set; } = string.Empty;
        public string[] ArrayRequiredPostData { get; set; }
        public string Chan { get; set; } = string.Empty;
        public string Guid { get; set; } = string.Empty;
        public string JsonP { get; set; } = "jsonp" + System.Guid.NewGuid().ToString().Replace("-", "");
        public string[] PagedListArray { get; set; }
        public string VconJson { get; set; } = string.Empty;
        public string NotifsNavItemBase { get; set; } = string.Empty;
        public string[] NotifsNavItemBaseArray { get; set; }
        public string ParentDomain { get; set; } = string.Empty;
        public string ReferringController { get; set; } = string.Empty;
        public string ReferringAction { get; set; } = string.Empty;
        public string ThreadId { get; set; } = string.Empty;
        public string Uid { get; set; } = string.Empty;
        public string TargetUid { get; set; } = string.Empty;
        public string ParentCid { get; set; } = string.Empty;
        public string ParentDomId { get; set; } = string.Empty;
        public string ForceCid { get; set; } = string.Empty;
        public string PageLoadUid { get; set; } = string.Empty;
        public string Hashes { get; set; } = string.Empty;
        public string SerializedComponent { get; set; } = string.Empty;
        public List<string> ThreadIds = new List<string>();
        #endregion
    }
}