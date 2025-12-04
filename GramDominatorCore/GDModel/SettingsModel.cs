using DominatorHouseCore.Utility;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class SettingsModel: BindableBase
    {
        //private bool _isEnableFollowDifferentUserChecked;
        //[ProtoMember(1)]
        //public bool IsEnableFollowDifferentUserChecked
        //{
        //    get
        //    {
        //        return _isEnableFollowDifferentUserChecked;
        //    }
        //    set
        //    {
        //        if (value == _isEnableFollowDifferentUserChecked)
        //            return;
        //        SetProperty(ref _isEnableFollowDifferentUserChecked, value);
        //    }
        //}

        //private bool _isEnableLikeDifferentUserChecked;
        //[ProtoMember(2)]
        //public bool IsEnableLikeDifferentUserChecked
        //{
        //    get
        //    {
        //        return _isEnableLikeDifferentUserChecked;
        //    }
        //    set
        //    {
        //        if (value == _isEnableLikeDifferentUserChecked)
        //            return;
        //        SetProperty(ref _isEnableLikeDifferentUserChecked, value);
        //    }
        //}
        //private bool _isEnableCommentDifferentUserChecked;
        //[ProtoMember(3)]
        //public bool IsEnableCommentDifferentUserChecked
        //{
        //    get
        //    {
        //        return _isEnableCommentDifferentUserChecked;
        //    }
        //    set
        //    {
        //        if (value == _isEnableCommentDifferentUserChecked)
        //            return;
        //        SetProperty(ref _isEnableCommentDifferentUserChecked, value);
        //    }
        //}

        //private bool _isMaxInstaConnectionsChecked;
        //[ProtoMember(4)]
        //public bool IsMaxInstaConnectionsChecked
        //{
        //    get
        //    {
        //        return _isMaxInstaConnectionsChecked;
        //    }
        //    set
        //    {
        //        if (value == _isMaxInstaConnectionsChecked)
        //            return;
        //        SetProperty(ref _isMaxInstaConnectionsChecked, value);
        //    }
        //}
        //private int _instaConnectionsValue;
        //[ProtoMember(5)]
        //public int InstaConnectionsValue
        //{
        //    get
        //    {
        //        return _instaConnectionsValue;
        //    }
        //    set
        //    {
        //        if (value == _instaConnectionsValue)
        //            return;
        //        SetProperty(ref _instaConnectionsValue, value);
        //    }
        //}
        //private bool _isEnableRepostDifferentUserChecked;
        //[ProtoMember(6)]
        //public bool IsEnableRepostDifferentUserChecked
        //{
        //    get
        //    {
        //        return _isEnableRepostDifferentUserChecked;
        //    }
        //    set
        //    {
        //        if (value == _isEnableRepostDifferentUserChecked)
        //            return;
        //        SetProperty(ref _isEnableRepostDifferentUserChecked, value);
        //    }
        //}
        //private bool _isEnablePostDifferentUserChecked;
        //[ProtoMember(7)]
        //public bool IsEnablePostDifferentUserChecked
        //{
        //    get
        //    {
        //        return _isEnablePostDifferentUserChecked;
        //    }
        //    set
        //    {
        //        if (value == _isEnablePostDifferentUserChecked)
        //            return;
        //        SetProperty(ref _isEnablePostDifferentUserChecked, value);
        //    }
        //}
        //private bool _isEnableMinimumDelayChecked;
        //[ProtoMember(8)]
        //public bool IsEnableMinimumDelayChecked
        //{
        //    get
        //    {
        //        return _isEnableMinimumDelayChecked;
        //    }
        //    set
        //    {
        //        if (value == _isEnableMinimumDelayChecked)
        //            return;
        //        SetProperty(ref _isEnableMinimumDelayChecked, value);
        //    }
        //}
        //private bool _isWaitBetweenMinimumDelayChecked;
        //[ProtoMember(9)]
        //public bool IsWaitBetweenMinimumDelayChecked
        //{
        //    get
        //    {
        //        return _isWaitBetweenMinimumDelayChecked;
        //    }
        //    set
        //    {
        //        if (value == _isWaitBetweenMinimumDelayChecked)
        //            return;
        //        SetProperty(ref _isWaitBetweenMinimumDelayChecked, value);
        //    }
        //}
        //private RangeUtilities _waitBetweenMinimumDelay = new RangeUtilities();
        //[ProtoMember(10)]
        //public RangeUtilities WaitBetweenMinimumDelay
        //{
        //    get
        //    {
        //        return _waitBetweenMinimumDelay;
        //    }
        //    set
        //    {
        //        if (value == _waitBetweenMinimumDelay)
        //            return;
        //        SetProperty(ref _waitBetweenMinimumDelay, value);
        //    }
        //}
        //private bool _isUpdateAccountNameChecked;
        //[ProtoMember(11)]
        //public bool IsUpdateAccountNameChecked
        //{
        //    get
        //    {
        //        return _isUpdateAccountNameChecked;
        //    }
        //    set
        //    {
        //        if (value == _isUpdateAccountNameChecked)
        //            return;
        //        SetProperty(ref _isUpdateAccountNameChecked, value);
        //    }
        //}
        //private bool _isDoNotUseEmbeddedBrowserToLoginChecked;
        //[ProtoMember(12)]
        //public bool IsDoNotUseEmbeddedBrowserToLoginChecked
        //{
        //    get
        //    {
        //        return _isDoNotUseEmbeddedBrowserToLoginChecked;
        //    }
        //    set
        //    {
        //        if (value == _isDoNotUseEmbeddedBrowserToLoginChecked)
        //            return;
        //        SetProperty(ref _isDoNotUseEmbeddedBrowserToLoginChecked, value);
        //    }
        //}
        //private bool _isDoNotUseEmbeddedBrowserToFollowChecked;
        //[ProtoMember(13)]
        //public bool IsDoNotUseEmbeddedBrowserToFollowChecked
        //{
        //    get
        //    {
        //        return _isDoNotUseEmbeddedBrowserToFollowChecked;
        //    }
        //    set
        //    {
        //        if (value == _isDoNotUseEmbeddedBrowserToFollowChecked)
        //            return;
        //        SetProperty(ref _isDoNotUseEmbeddedBrowserToFollowChecked, value);
        //    }
        //}

        //private bool _isAutoPhoneVerifyAccountChecked;
        //[ProtoMember(14)]
        //public bool IsAutoPhoneVerifyAccountChecked
        //{
        //    get
        //    {
        //        return _isAutoPhoneVerifyAccountChecked;
        //    }
        //    set
        //    {
        //        if (value == _isAutoPhoneVerifyAccountChecked)
        //            return;
        //        SetProperty(ref _isAutoPhoneVerifyAccountChecked, value);
        //    }
        //}
        //private bool _isWaitBetweenVerifyAccountChecked;
        //[ProtoMember(15)]
        //public bool IsWaitBetweenVerifyAccountChecked
        //{
        //    get
        //    {
        //        return _isWaitBetweenVerifyAccountChecked;
        //    }
        //    set
        //    {
        //        if (value == _isWaitBetweenVerifyAccountChecked)
        //            return;
        //        SetProperty(ref _isWaitBetweenVerifyAccountChecked, value);
        //    }
        //}
        //private RangeUtilities _waitBetweenVerifyAccount = new RangeUtilities();
        //[ProtoMember(16)]
        //public RangeUtilities WaitBetweenVerifyAccount
        //{
        //    get
        //    {
        //        return _waitBetweenVerifyAccount;
        //    }
        //    set
        //    {
        //        if (value == _waitBetweenVerifyAccount)
        //            return;
        //        SetProperty(ref _waitBetweenVerifyAccount, value);
        //    }
        //}

        //private bool _isAutoLoginAfterPhoneVerificationChecked;
        //[ProtoMember(17)]
        //public bool IsAutoLoginAfterPhoneVerificationChecked
        //{
        //    get
        //    {
        //        return _isAutoLoginAfterPhoneVerificationChecked;
        //    }
        //    set
        //    {
        //        if (value == _isAutoLoginAfterPhoneVerificationChecked)
        //            return;
        //        SetProperty(ref _isAutoLoginAfterPhoneVerificationChecked, value);
        //    }
        //}

        //private bool _isEnableDelayForBulkChecked;
        //[ProtoMember(18)]
        //public bool IsEnableDelayForBulkChecked
        //{
        //    get
        //    {
        //        return _isEnableDelayForBulkChecked;
        //    }
        //    set
        //    {
        //        if (value == _isEnableDelayForBulkChecked)
        //            return;
        //        SetProperty(ref _isEnableDelayForBulkChecked, value);
        //    }
        //}



        //private bool _isWaitBetweenBulkDelayChecked;
        //[ProtoMember(19)]
        //public bool IsWaitBetweenBulkDelayChecked
        //{
        //    get
        //    {
        //        return _isWaitBetweenBulkDelayChecked;
        //    }
        //    set
        //    {
        //        if (value == _isWaitBetweenBulkDelayChecked)
        //            return;
        //        SetProperty(ref _isWaitBetweenBulkDelayChecked, value);
        //    }
        //}
        //private RangeUtilities _waitBetweenBulkDelay = new RangeUtilities();
        //[ProtoMember(20)]
        //public RangeUtilities WaitBetweenBulkDelay
        //{
        //    get
        //    {
        //        return _waitBetweenBulkDelay;
        //    }
        //    set
        //    {
        //        if (value == _waitBetweenBulkDelay)
        //            return;
        //        SetProperty(ref _waitBetweenBulkDelay, value);
        //    }
        //}
        //private bool _isDisableAutoReportProblemChecked;
        //[ProtoMember(21)]
        //public bool IsDisableAutoReportProblemChecked
        //{
        //    get
        //    {
        //        return _isDisableAutoReportProblemChecked;
        //    }
        //    set
        //    {
        //        if (value == _isDisableAutoReportProblemChecked)
        //            return;
        //        SetProperty(ref _isDisableAutoReportProblemChecked, value);
        //    }
        //}
        //private bool _isDisableOptimizationForGettingUserInfoChecked;
        //[ProtoMember(22)]
        //public bool IsDisableOptimizationForGettingUserInfoChecked
        //{
        //    get
        //    {
        //        return _isDisableOptimizationForGettingUserInfoChecked;
        //    }
        //    set
        //    {
        //        if (value == _isDisableOptimizationForGettingUserInfoChecked)
        //            return;
        //        SetProperty(ref _isDisableOptimizationForGettingUserInfoChecked, value);
        //    }
        //}
        //private bool _isApiToSaveBandwidthChecked;
        //[ProtoMember(23)]
        //public bool IsApiToSaveBandwidthChecked
        //{
        //    get
        //    {
        //        return _isApiToSaveBandwidthChecked;
        //    }
        //    set
        //    {
        //        if (value == _isApiToSaveBandwidthChecked)
        //            return;
        //        SetProperty(ref _isApiToSaveBandwidthChecked, value);
        //    }
        //}
        //private bool _isStopAllAccountsIfMoreThanChecked;
        //[ProtoMember(24)]
        //public bool IsStopAllAccountsIfMoreThanChecked
        //{
        //    get
        //    {
        //        return _isStopAllAccountsIfMoreThanChecked;
        //    }
        //    set
        //    {
        //        if (value == _isStopAllAccountsIfMoreThanChecked)
        //            return;
        //        SetProperty(ref _isStopAllAccountsIfMoreThanChecked, value);
        //    }
        //}

        //private RangeUtilities _waitBetweenStopAllAccounts = new RangeUtilities();
        //[ProtoMember(25)]
        //public RangeUtilities WaitBetweenStopAllAccounts
        //{
        //    get
        //    {
        //        return _waitBetweenStopAllAccounts;
        //    }
        //    set
        //    {
        //        if (value == _waitBetweenStopAllAccounts)
        //            return;
        //        SetProperty(ref _waitBetweenStopAllAccounts, value);
        //    }
        //}
        //private bool _isStopPhoneVerificationChecked;
        //[ProtoMember(26)]
        //public bool IsStopAllPhoneVerificationChecked
        //{
        //    get
        //    {
        //        return _isStopPhoneVerificationChecked;
        //    }
        //    set
        //    {
        //        if (value == _isStopPhoneVerificationChecked)
        //            return;
        //        SetProperty(ref _isStopPhoneVerificationChecked, value);
        //    }
        //}

        //private RangeUtilities _phoneVerification = new RangeUtilities();
        //[ProtoMember(27)]
        //public RangeUtilities PhoneVerification
        //{
        //    get
        //    {
        //        return _phoneVerification;
        //    }
        //    set
        //    {
        //        if (value == _phoneVerification)
        //            return;
        //        SetProperty(ref _phoneVerification, value);
        //    }
        //}

        //private bool _isTreatActionBlockTemporaryBlocksChecked;
        //[ProtoMember(28)]
        //public bool IsTreatActionBlockTemporaryBlocksChecked
        //{
        //    get
        //    {
        //        return _isTreatActionBlockTemporaryBlocksChecked;
        //    }
        //    set
        //    {
        //        if (value == _isTreatActionBlockTemporaryBlocksChecked)
        //            return;
        //        SetProperty(ref _isTreatActionBlockTemporaryBlocksChecked, value);
        //    }
        //}
        //private bool _isSupspendToolWhenTemporaryBlockedChecked;
        //[ProtoMember(29)]
        //public bool IsSupspendToolWhenTemporaryBlockedChecked
        //{
        //    get
        //    {
        //        return _isSupspendToolWhenTemporaryBlockedChecked;
        //    }
        //    set
        //    {
        //        if (value == _isSupspendToolWhenTemporaryBlockedChecked)
        //            return;
        //        SetProperty(ref _isSupspendToolWhenTemporaryBlockedChecked, value);
        //    }
        //}

        //private RangeUtilities _temporaryBlockedBetween = new RangeUtilities();
        //[ProtoMember(30)]
        //public RangeUtilities TemporaryBlockedBetween
        //{
        //    get
        //    {
        //        return _temporaryBlockedBetween;
        //    }
        //    set
        //    {
        //        if (value == _temporaryBlockedBetween)
        //            return;
        //        SetProperty(ref _temporaryBlockedBetween, value);
        //    }
        //}

    }
}
