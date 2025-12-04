using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumblr.Enum
{
    public enum TumblrError
    {
        CheckPoint,
        FailedRequest,
        ToManyActions,
        SettingsCritical,
        FollowSettingsMinor,
        RateLimit,
        IncorrectPassword,
        NotAuthorized,
        UsernameNotExist,
        NotFound,
        SentryBlocked,
        LoginRequired,
        PasswordReset,
        AccountDisabled,
        Challenge,
        ScrapingMinor,
        CanNotLike,
        InputError,
        Feedback,
        TwoFactor,
    }
}
