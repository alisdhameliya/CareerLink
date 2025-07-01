public interface IUserSkills
{
    Task<int> AddUserSkills(t_UserSkills skills);
    Task<List<t_UserSkills>> GetUserSkills(int id);
    Task<int> UpdateUserSkills(t_UserSkills t_UserSkills);

    Task<int> DeleteUserSkills(int id);

    Task<t_UserSkills> GetOneSkill(int id);
}