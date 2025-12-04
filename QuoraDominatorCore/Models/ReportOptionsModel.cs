using DominatorHouseCore.Utility;

namespace QuoraDominatorCore.Models
{
    public class ReportSubOption
    {
        public string Title {  get; set; }
        public string Description { get; set; }
        public bool HaveSubOption {  get; set; }
    }
    public class ReportOptionsModel:BindableBase
    {
        private string _title=string.Empty;
        private string _description=string.Empty;
        private bool haveTitle=true;
        private bool haveDescription=false;
        public string ReportDescription { get;set; }=string.Empty;
        public string ReportOptionTitle { get;set; }= string.Empty;
        private ReportSubOption _SubOption = new ReportSubOption();
        public ReportSubOption SubOption
        {
            get => _SubOption;
            set => SetProperty(ref _SubOption,value);
        }
        public string Title
        {
            get { return _title; }
            set
            {
               if (value == _title) return;
               SetProperty(ref _title, value);
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                SetProperty(ref _description, value);
            }
        }
        public bool HasTitle
        {
            get { return haveTitle; }
            set
            {
                if (value == haveTitle) return;
                SetProperty(ref haveTitle, value);
            }
        }
        public bool HasDescription
        {
            get { return haveDescription; }
            set
            {
                if (value == haveDescription) return;
                SetProperty(ref haveDescription, value);
            }
        }
    }
}
