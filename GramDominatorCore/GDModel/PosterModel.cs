using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using ProtoBuf;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class PosterModel : ModuleSetting, IGeneralSettings
    {
      

        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        //private bool _isSinglePostChecked=true;
        //[ProtoMember(2)]
        //public bool IsSinglePostChecked
        //{
        //    get
        //    {
        //        return _isSinglePostChecked;
        //    }
        //    set
        //    {
        //        if (value == _isSinglePostChecked)
        //            return;
        //        SetProperty(ref _isSinglePostChecked, value);
        //    }
        //}
        //private bool _isMultiPostChecked;
        //[ProtoMember(3)]
        //public bool IsMultiPostChecked
        //{
        //    get
        //    {
        //        return _isMultiPostChecked;
        //    }
        //    set
        //    {
        //        if (value == _isMultiPostChecked)
        //            return;
        //        SetProperty(ref _isMultiPostChecked, value);
        //    }
        //}

        //private bool _isMonitoredFolderChecked;
        //[ProtoMember(3)]
        //public bool IsMonitoredFolderChecked
        //{
        //    get
        //    {
        //        return _isMonitoredFolderChecked;
        //    }
        //    set
        //    {
        //        if (value == _isMonitoredFolderChecked)
        //            return;
        //        SetProperty(ref _isMonitoredFolderChecked, value);
        //    }
        //}

        //private int _threadCount;
        //[ProtoMember(4)]
        //public int ThreadCount
        //{
        //    get
        //    {
        //        return _threadCount;
        //    }
        //    set
        //    {
        //        if (value == _threadCount)
        //            return;
        //        SetProperty(ref _threadCount, value);
        //    }
        //}
        //private bool _isUploadAsNormalPicChecked;
        //[ProtoMember(5)]
        //public bool IsUploadAsNormalPicChecked
        //{
        //    get
        //    {
        //        return _isUploadAsNormalPicChecked;
        //    }
        //    set
        //    {
        //        if (value == _isUploadAsNormalPicChecked)
        //            return;
        //        SetProperty(ref _isUploadAsNormalPicChecked, value);
        //    }
        //}
        //private bool _isUploadAsProfilePicChecked;
      
        //[ProtoMember(6)]
        //public bool IsUploadAsProfilePicChecked
        //{
        //    get
        //    {
        //        return _isUploadAsProfilePicChecked;
        //    }
        //    set
        //    {
        //        if (value == _isUploadAsProfilePicChecked)
        //            return;
        //        SetProperty(ref _isUploadAsProfilePicChecked, value);
        //    }
        //}
        //private RangeUtilities _delayBetweenJobs=new RangeUtilities();
        //[ProtoMember(7)]
        //public RangeUtilities DelayBetweenJobs
        //{
        //    get
        //    {
        //        return _delayBetweenJobs;
        //    }
        //    set
        //    {
        //        if (value == _delayBetweenJobs)
        //            return;
        //        SetProperty(ref _delayBetweenJobs, value);
        //    }
        //}

        //private RangeUtilities _delayBetweenAccount = new RangeUtilities();
        //[ProtoMember(8)]
        //public RangeUtilities DelayBetweenAccount
        //{
        //    get
        //    {
        //        return _delayBetweenAccount;
        //    }
        //    set
        //    {
        //        if (value == _delayBetweenAccount)
        //            return;
        //        SetProperty(ref _delayBetweenAccount, value);
        //    }
        //}

        //private int _totalActionCount;
        //[ProtoMember(9)]
        //public int TotalActionCount
        //{
        //    get
        //    {
        //        return _totalActionCount;
        //    }
        //    set
        //    {
        //        if (value == _totalActionCount)
        //            return;
        //        SetProperty(ref _totalActionCount, value);
        //    }
        //}

        //private bool _isUniqueOperationChecked;
        //[ProtoMember(3)]
        //public bool IsUniqueOperationChecked
        //{
        //    get
        //    {
        //        return _isUniqueOperationChecked;
        //    }
        //    set
        //    {
        //        if (value == _isUniqueOperationChecked)
        //            return;
        //        SetProperty(ref _isUniqueOperationChecked, value);
        //    }
        //}

    }
}
