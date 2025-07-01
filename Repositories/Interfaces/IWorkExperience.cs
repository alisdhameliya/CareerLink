public interface IWorkExperience
{
    Task<int> AddWorkExperience(t_Work_Experience experience);

    Task<List<t_Work_Experience>> GetWorkExperience(int id);
    
    Task<int> UpdateWorkExperience(t_Work_Experience experience);

    Task<int> DeleteWorkExperience(int id);

    Task<t_Work_Experience> GetOneWorkExperience(int id);
}