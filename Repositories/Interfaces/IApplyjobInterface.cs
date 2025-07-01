using Repositories.Model;

public interface IApplyjobInterface
{
     Task<FieldCheckResult?> CheckFields(int id);

    Task<int> ApplyJob(t_apply_job apply_Job);

    // Task<int> DeleteJob(int id);

    Task<List<t_apply_job>> GetApplyJobApplication(int id,string job_title,string status);

    Task<int> UpdateStatusOfJobApplication(t_apply_job apply_Job);

    Task<List<t_apply_job>> GetUserAppliedJobApplication(int id);

    Task<List<Job_Post>> GetJobTitles(int id);

    //Interview Schedule starts from here
    Task<int> InterviewSchedule(t_interview_schedule interview_Schedule);

    Task<List<t_interview_schedule>> GetInterviews_ScheduleByCompany(int id);

    Task<int> UpdateInterviewSchedule(t_interview_schedule interview_Schedule);

    Task<int> UserAppliedStatus(int userid,int jobid);

}