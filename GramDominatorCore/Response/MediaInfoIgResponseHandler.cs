using System.Linq;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.Response
{
    public class MediaInfoIgResponseHandler : IGResponseHandler
    {
        public MediaInfoIgResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                if (!Success)
                    return;
                if (Success && response.Response.Contains("<!DOCTYPE html>"))
                {
                    return;
                }
                var DataList = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "items"));
                if (DataList != null && DataList.HasValues)
                {
                    InstagramPost = DataList.GetImageData().FirstOrDefault();
                }
            }
            catch (System.Exception ex)
            {
                ex.DebugLog();
            }
        }

        public InstagramPost InstagramPost { get; set; }
    }
}
