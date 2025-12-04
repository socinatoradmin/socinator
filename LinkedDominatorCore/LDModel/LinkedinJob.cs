using DominatorHouseCore.Interfaces;

namespace LinkedDominatorCore.LDModel
{
    public class LinkedinJob : IJob
    {
        public LinkedinJob(string JobPostId)
        {
            this.JobPostId = JobPostId;
            JobPostUrl = $"https://www.linkedin.com/jobs/view/{JobPostId}";
        }

        public LinkedinJob()
        {
        }


        public string JobTitle { get; set; }
        public string JobPostId { get; set; }
        public string JobPostUrl { get; set; }
        public string JobPosterID { get; set; }


        //Note Check DB model For InteractedJobs and add other Properties as Reqired
    }
}