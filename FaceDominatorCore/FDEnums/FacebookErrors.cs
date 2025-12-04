using System.ComponentModel;

namespace FaceDominatorCore.FDEnums
{
    public enum FacebookErrors
    {

        [Description("Verfication required")]
        CheckPoint = 1,

        [Description("Account disabled")]
        AccountDisbled = 2,

        [Description("Invalid username or password")]
        InvalidLogin = 3,

        /*
                [Description("The session is invalid because the user logged out")]
                InvalidSession = 4,
        */

        [Description("An unknown error occurred")]
        UnknownErrorOccurred = 5,

        /*
                [Description("Service temporarily unavailable")]
                ServiceUnavailable = 6,
        */

        /*
                [Description("Application request limit reached")]
                RequestLimitReached = 7,
        */

        /*
                [Description("This method requires an HTTPS connection")]
                SecureConnectionRequired = 8,
        */

        /*
                [Description("User is performing too many actions")]
                TooManyActions = 9,
        */

        /*
                [Description("Account does not have permission for this action")]
                PermissionDenied = 10,
        */

        [Description("Sorry, something went wrong. Please try again later.")]
        Failed = 11,

        /*
                [Description("Comment not found")]
                NotFoundComment = 12,
        */

        /*
                [Description("Invalid parameter")]
                InvalidParameter = 13,
        */

        /*
                [Description("Please enter a valid email address")]
                InvalidEmail = 14,
        */

        /*
                [Description("url should represent a valid URL")]
                InvalidUrl = 15,
        */

        /*
                [Description("Must be a valid ISO 4217 currency code")]
                InvalidCode = 16,
        */

        /*
                [Description("Incorrect signature")]
                IncorrectSignature = 17,
        */

        /*
                [Description("The number of parameters exceeded the maximum for this operation")]
                TooManyParameter = 18,
        */

        /*
                [Description("Invalid album id")]
                InvalidAlbumId = 19,
        */

        /*
                [Description("Invalid photo id")]
                InvalidPhotoId = 20,
        */

        /*
                [Description("Invalid category")]
                InvalidCategory = 21,
        */

        /*
                [Description("Invalid subcategory")]
                InvalidSubCategory = 22,
        */

        /*
                [Description("The images you tried to upload are too many")]
                TooManyImagesUploaded = 23,
        */

        /*
                [Description("Message contains banned content")]
                MessageBanned = 24,
        */

        /*
                [Description("Missing message body")]
                NoMessageBody = 25,
        */

        /*
                [Description("Message is too long")]
                TooLongMessage = 26,
        */

        /*
                [Description("User has sent too many messages")]
                MessageLimitExceed = 27,
        */

        /*
                [Description("Invalid message recipient")]
                InvalidRecipient = 28,
        */

        /*
                [Description("An error occurred while sending the message.")]
                ErrorSendingMessage = 29,
        */

        /*
                [Description("Login Required")]
                LoginRequired = 30,
        */

        [Description("The privacy setting on this post means that you can't share it.")]
        PostPrivacySetting = 31,

        [Description("Proxy Not Working")]
        ProxyNotWorking = 31,

        [Description("Activity Blocked By Facebook")]
        ActivityBlocked = 32,

        /*
                [Description("Dont have permission to publish")]
                DontHavePermissionToPublish = 33,
        */

        [Description("Page Dosen't Allow Visitor Pot")]
        PageDosentAllowVisitorPot = 34


    }
}