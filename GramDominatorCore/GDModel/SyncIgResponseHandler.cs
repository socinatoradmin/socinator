using System.Collections.Generic;
using System.ComponentModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.GDModel
{
    [Localizable(false)]
    public class SyncIgResponseHandler : GDLibrary.Response.IGResponseHandler
    {
        public SyncIgResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            //foreach (JToken jtoken1 in (JArray)RespJ["layout"]["bloks_payload"])
            //{
            //    if (jtoken1["name"] != null && jtoken1["params"] != null)
            //    {
            //        string key = jtoken1["name"].ToString();
            //        if (!Experiments.ContainsKey(key))
            //            Experiments.Add(key, new Dictionary<string, string>());
            //        foreach (JToken jtoken2 in jtoken1["params"])
            //        {
            //            if (jtoken2["name"] != null)
            //                Experiments[key].Add(jtoken2["name"].ToString(), jtoken2["value"].ToString());
            //        }
            //    }
            //}
            JsonHandler jsonHandle = new JsonHandler(response.Response);

            var page_layout_details = jsonHandle.GetElementValue("layout", "bloks_payload", "embedded_payloads", 6, "payload", "layout", "bloks_payload", "tree", "㜍", "$", "㐈", " ", 0, "㐈", " ", 0, "㐈", " ", 0, "㐈", " ", 1, "㐈", " ", 4, "㐈", " ", 2, "㐈", " ", 0, "㐈", "\u0085", 1, "㚝", "&");

            try
            {
                var screenResponse = Utilities.GetBetween(response.Response, "(bk.action.array.Make, \\\"com.bloks.www.caa.login.cp_text_input_type_ahead\\\"))), ", "))), (bk.action.map.Make, (bk.action.array.Make, \\\"account_centers\\\", \\\"query\\\")");
                var splittedItemsInText = System.Text.RegularExpressions.Regex.Split(screenResponse,"bk.action.i64.Const");
                if(splittedItemsInText.Length<2)
                {
                    screenResponse = Utilities.GetBetween(response.Response, "com.bloks.www.caa.login.cp_text_input_type_ahead\\\", ",", (bk.action.map.Make, (bk.action.array.Make, \\\"account_centers\\\", \\\"query\\\")");
                    splittedItemsInText = System.Text.RegularExpressions.Regex.Split(screenResponse, "bk.action.i64.Const");
                }
                var extractData = Utilities.GetBetween(page_layout_details, "Email or mobile number required", "com.bloks.www.bloks.caa.login.async.send_login_request");
                if(splittedItemsInText.Length>5)
                {
                    var screenId = Utilities.GetBetween(splittedItemsInText[1],",",")").Trim();
                    var componentId = Utilities.GetBetween(splittedItemsInText[2], ",", ")").Trim();
                    var textInputId = Utilities.GetBetween(splittedItemsInText[3], ",", ")").Trim();
                    var typeAheadId = Utilities.GetBetween(splittedItemsInText[4], ",", ")").Trim();
                    var fdid = Utilities.GetBetween(splittedItemsInText[4], "\\\"", "\\\"").Trim();
                    var textInstanceId = splittedItemsInText[5].Split(',')[1].Trim();
                    Experiments.Add("screen_id", screenId);
                    Experiments.Add("component_id", componentId);
                    Experiments.Add("text_input_id", textInputId);
                    Experiments.Add("type_ahead_id", typeAheadId);
                    Experiments.Add("fdid", fdid);
                    Experiments.Add("text_instance_id", textInputId);
                }
                var arrayData = System.Text.RegularExpressions.Regex.Split(extractData, "bk.action.bloks.Find,");
                var listOfData = new List<string>();
                if (arrayData.Length > 6)
                {
                    var username_id = Utilities.GetBetween(arrayData[2], "\"", "\"");
                    var password_id = Utilities.GetBetween(arrayData[5], "\"", "\"");//103025064300235
                    Experiments.Add("username_id", username_id);
                    Experiments.Add("password_id", password_id);

                }

                var instanceId = Utilities.GetBetween(extractData, "(bk.action.qpl.MarkerStartV2, (bk.action.i32.Const, 36707139), (bk.action.i32.Const,", ")").Trim();
                var markerId = Utilities.GetBetween(extractData, ", \"CAA_LOGIN_FORM:is_login_pending:0\", (bk.action.bool.Const, true)), (bk.action.core.TakeLast, (bk.action.qpl.MarkerStartV2, (bk.action.i32.Const,", ")").Trim();
                Experiments.Add("marker_id", markerId);
                Experiments.Add("instance_id", instanceId);
            }
            catch (System.Exception)
            {

            }
        }

        public Dictionary<string, string> Experiments { get; set; } = new Dictionary<string, string>();
    }
}
