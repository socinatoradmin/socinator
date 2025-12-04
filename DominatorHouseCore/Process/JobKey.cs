namespace DominatorHouseCore.Process
{
    public struct JobKey
    {
        public string AccountId { get; }
        public string TemplateId { get; }

        public JobKey(string accountId, string templateId)
        {
            AccountId = accountId;
            TemplateId = templateId;
        }

        public static implicit operator string(JobKey x)
        {
            return $"{x.AccountId}-------{x.TemplateId}";
        }
    }
}