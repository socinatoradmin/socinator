using DominatorHouseCore;
using DominatorHouseCore.Utility;
using System;
using YoutubeDominatorCore.YoutubeModels;

namespace YoutubeDominatorCore.Response
{
    public class ReportVideoResponseHandler
    {
        public ReportVideoResponseHandler()
        {
        }

        public ReportVideoResponseHandler(string response, int hitStep, int reportOption = 0, int reportSubOption = 0)
        {
            try
            {
                var jsonHand = new JsonHandler(response);
                reportSubOption = reportSubOption + 1;
                switch (hitStep)
                {
                    case 1:
                        PostDataElements = new PostDataElements();
                        var subOption = string.Empty;
                        if (reportOption == 4 || reportOption == 5)
                        {
                            PostDataElements.ClickTrackingParams = jsonHand.GetElementValue("data", "actions", 0,
                                "openPopupAction", "popup", "reportFormModalRenderer", "optionsSupportedRenderers",
                                "optionsRenderer", "items", reportOption, "optionSelectableItemRenderer",
                                "trackingParams");
                            PostDataElements.FlagAction = jsonHand.GetElementValue("data", "actions", 0,
                                "openPopupAction", "popup", "reportFormModalRenderer", "optionsSupportedRenderers",
                                "optionsRenderer", "items", reportOption, "optionSelectableItemRenderer",
                                "submitEndpoint", "flagEndpoint", "flagAction");
                            PostDataElements.FlagRequestType = jsonHand.GetElementValue("data", "actions", 0,
                                "openPopupAction", "popup", "reportFormModalRenderer", "optionsSupportedRenderers",
                                "optionsRenderer", "items", reportOption, "optionSelectableItemRenderer",
                                "submitEndpoint", "flagEndpoint", "flagRequestType");
                        }
                        else
                        {
                            PostDataElements.ClickTrackingParams = jsonHand.GetElementValue("data", "actions", 0,
                                "openPopupAction", "popup", "reportFormModalRenderer", "optionsSupportedRenderers",
                                "optionsRenderer", "items", reportOption, "optionSelectableItemRenderer", "subOptions",
                                reportSubOption, "optionSelectableItemRenderer", "trackingParams");
                            PostDataElements.FlagAction = jsonHand.GetElementValue("data", "actions", 0,
                                "openPopupAction", "popup", "reportFormModalRenderer", "optionsSupportedRenderers",
                                "optionsRenderer", "items", reportOption, "optionSelectableItemRenderer", "subOptions",
                                reportSubOption, "optionSelectableItemRenderer", "submitEndpoint", "flagEndpoint",
                                "flagAction");
                            PostDataElements.FlagRequestType = jsonHand.GetElementValue("data", "actions", 0,
                                "openPopupAction", "popup", "reportFormModalRenderer", "optionsSupportedRenderers",
                                "optionsRenderer", "items", reportOption, "optionSelectableItemRenderer", "subOptions",
                                reportSubOption, "optionSelectableItemRenderer", "submitEndpoint", "flagEndpoint",
                                "flagRequestType");
                            subOption = jsonHand.GetElementValue("data", "actions", 0, "openPopupAction", "popup",
                                "reportFormModalRenderer", "optionsSupportedRenderers", "optionsRenderer", "items",
                                reportOption, "optionSelectableItemRenderer", "subOptions", reportSubOption,
                                "optionSelectableItemRenderer", "text", "simpleText");
                        }

                        var option = jsonHand.GetElementValue("data", "actions", 0, "openPopupAction", "popup",
                            "reportFormModalRenderer", "optionsSupportedRenderers", "optionsRenderer", "items",
                            reportOption, "optionSelectableItemRenderer", "text", "simpleText");

                        SelectedOptionToVideoReport =
                            !string.IsNullOrEmpty(subOption) ? $"{option} [{subOption}]" : option;
                        break;
                    case 2:
                        PostDataElements = new PostDataElements();
                        PostDataElements.ClickTrackingParams = jsonHand.GetElementValue("data", "actions", 0,
                            "openPopupAction", "popup", "reportDetailsFormRenderer", "submitButton", "buttonRenderer",
                            "serviceEndpoint", "clickTrackingParams");
                        break;
                    case 3:
                        Success = jsonHand.GetElementValue("code").ToUpper() == "SUCCESS";
                        if (!Success)
                        {
                            /*just to check*/
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public PostDataElements PostDataElements { get; set; }
        public bool Success { get; set; }
        public string SelectedOptionToVideoReport { get; set; }
    }
}