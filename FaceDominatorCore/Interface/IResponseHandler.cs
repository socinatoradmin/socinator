using FaceDominatorCore.FDLibrary.FdClassLibrary;

namespace FaceDominatorCore.Interface
{
    public interface IResponseHandler : ICommonResponseParam
    {
        FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

    }
    public interface ICommonResponseParam
    {
        string EntityId { get; set; }

        string PageletData { get; set; }

        bool HasMoreResults { get; set; }

        bool Status { get; set; }
    }
}
