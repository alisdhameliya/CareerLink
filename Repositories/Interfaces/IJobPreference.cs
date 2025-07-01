public interface IJobPreference
{
    Task<int> AddJobPreference(t_JobPreference preference);

    Task<t_JobPreference> GetJobPreference(int id);

    Task<int> UpdateJobPreference(t_JobPreference preference);

    Task<int> DeleteJobPreference(int id);

    Task<t_JobPreference> GetOneJobPreference(int id);   
}