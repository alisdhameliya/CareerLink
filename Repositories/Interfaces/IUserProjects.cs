public interface IUserProjects
{
    Task<int> AddUserProjects(t_UserProjects projects);

    Task<List<t_UserProjects>> GetUserProjects(int id);

    Task<int> UpdateUserProjects(t_UserProjects projects);
    Task<int> DeleteUserProjects(int id);

    Task<t_UserProjects> GetOneProject(int id);
}