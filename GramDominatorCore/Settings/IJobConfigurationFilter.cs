namespace GramDominatorCore.Settings
{

    /// <summary> 
    /// Dont Use this, Instead use  DominatorHouseCore.Interfaces.IJobConfiguration
    /// </summary>
    interface IJobConfigurationFilter
    {
      
        // Specify the operation count for the job in numbers
        // RangeUtilities OperationCountPerJob { get; set; }
      
        // Specify the delay range for the jobs in minutes
       //  RangeUtilities DelayBetweenJobs { get; set; }
     

       
        // Specify the delay range for the operations in seconds
        // RangeUtilities DelayBetweenOperation { get; set; }
       

        // Specify the current job configurations are enable or not  
      
        // bool IsJobConfigEnabled { get; set; }


     
        // Specify the operation count per day in numbers
       //  RangeUtilities OperationCountPerDay { get; set; }

 
        // Specify the operation count per hours in numbers
        // RangeUtilities OperationCountPerHour { get; set; }


       
        // Specify the operation count per week in numbers
        // RangeUtilities OperationCountPerWeek { get; set; } 

     
        // Whether the current job is paused or not
       //  bool PauseCurrentJob { get; set; }
        // Specify the timespan value to untill the job is paused
      //  int PauseCurrentJobUntill { get; set; }


    
        // Specify the restricted user list
       //  BlacklistSettings RestrictedUserList { get; set; }


      
        // Specify the restricted group list
       //  BlacklistSettings RestrictedGroupList { get; set; }


      
        // Specify the running timespam for the job 
       //  List<RunningTimeSpanModel> RunningTimeSpan { get; set; }
         //RunningTimeSpanModel RunningTimeSpan { get; set; }

      
        // Is the job is going for next schedule in the day, verify the max hour or day or week count is already crossed or not
       //  bool ShouldSchedule { get; set; }


        // Specify the current status of the job
       // string JobCurrentStatus { get; }
    }
}
