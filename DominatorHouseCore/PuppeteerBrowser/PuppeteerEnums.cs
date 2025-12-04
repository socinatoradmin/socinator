using System.ComponentModel;

namespace DominatorHouseCore.PuppeteerBrowser
{
    public enum ActType
    {
        [Description("click()")] Click = 1,
        ClickById = 2,
        ClickByName = 3,
        ClickByClass = 4,
        EnterValue = 5,
        EnterByQuery = 6,
        EnterValueById = 7,
        EnterValueByName = 8,
        EnterValueByClass = 9,
        [Description("focus()")] Focus = 10,
        ActByQuery = 11,
        CustomActType = 12,
        CustomActByQueryType = 13,
        GetValue = 14,
        GetLength = 15,
        GetAttribute = 16,
        GetValueByName = 17,
        GetLengthByQuery = 18,
        GetLengthByClass = 19,
        GetValueByTagName = 20,
        GetLengthByCustomQuery = 21,
        ScrollWindow = 22,
        ScrollIntoView = 23,
        ScrollIntoViewQuery = 24,
        ScrollByQuery = 25,
        ScrollIntoViewChildQuery = 26,
        GetLengthByCustomAttributes = 27
    }

    public enum ValueTypes
    {
        [Description("id")] Id = 1,
        [Description("innerHTML")] InnerHtml = 2,
        [Description("outerHTML")] OuterHtml = 3,
        [Description("innerText")] InnerText = 4,
        [Description("outerText")] OuterText = 5,
        [Description("parentElement.id")] ParentId = 6,
        [Description("href")] Href = 7,
        [Description("data-timestamp")] TimeStamp = 8,
        [Description("src")] Source = 9,
        [Description("aria-pressed")] AriaPressed = 10,
        [Description("aria-label")] AriaLabel = 11,
        [Description("placeholder")] Placeholder = 12,
        [Description("dataset.id")] DataId = 13,
        [Description("ariaSelected")] AreaSelected = 14
    }

    public enum MouseClickType
    {
        Left = 1,
        Right = 2,
        Middle = 3
    }

    public enum PostContent
    {
        PostId = 1,
        AdId = 2,
        PostUrl = 3,
        PostedTime = 4,
        LikerCount = 5,
        ShareCount = 6,
        CommentCount = 7,
        OwnerDetails = 8,
        OwnerLogo = 9,
        AdDetails = 10,
        AdText = 11,
        NewsFeedDescription = 12,
        SubDescription = 13,
        CallToAction = 14,
        Title = 15,
        SharedPostText = 16,
        SharedPostUrl = 17,
        SharedAdTitle = 18,
        MediaDetails = 19,
        IsSharePost = 20,
        MediaList = 21
    }

    public enum CoordinateDirection
    {
        [Description("left")] Left = 1,
        [Description("right")] Right = 2,
        [Description("top")] Top = 3,
        [Description("bottom")] Buttom = 4
    }

    public enum AttributeType
    {
        Null = 0,
        [Description("id")] Id = 1,
        [Description("name")] Name = 2,
        ClassName = 3,
        TagName = 4,
        [Description("value")] Value = 5,
        [Description("data-testid")] DataTestId = 6,
        [Description("role")] Role = 7,
        [Description("data-comment-prelude-ref")] CommentPreclude = 8,
        [Description("data-feed-option-name")] DataFeedOptionName = 9,
        [Description("title")] Title = 10,
        [Description("data-tab-key")] DataTabKey = 11,
        [Description("data-target")] DataTarget = 12,
        [Description("data-key")] DataKey = 13,
        [Description("data-referrer")] DataReferer = 14,
        [Description("type")] Type = 15,
        [Description("data-click")] DataClick = 16,
        [Description("aria-checked")] AriaChecked = 17,
        [Description("aria-label")] AriaLabel = 18,
        [Description("action_click")] ActionClick = 19,
        [Description("target")] Target = 20,
        [Description("loggingname")] LoggingName = 21,
        [Description("action_mousedown")] ActionMouseDown = 22,
        [Description("aria-relevant")] AriaRelevant = 23,
        [Description("data-bloks-name")] DataBlocksName = 24,
        [Description("jsname")] Jsname = 25,
        [Description("data-errormessage")] DataErrormessage = 26,
        [Description("data-pagelet")] Datapagelet = 27,
        [Description("data-sigil")] DataSigil = 28,
        [Description("aria-selected")] AriaSelected = 29
    }
}