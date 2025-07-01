public interface IUserResume
{
    Task<int> AddUserResume(t_UserResume resume);

    Task<int> UpdateUserResume(t_UserResume resume);

    Task<int> DeleteUserResume(int id);

    Task<t_UserResume> GetUserResume(int id);

    Task<t_UserResume> GetOneResume(int id);
}