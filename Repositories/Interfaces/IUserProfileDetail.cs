public interface IUserProfileDetail
{
    Task<t_UserProfileDetail> GetUserProfileDetail(int id);

    Task<t_UserProfileDetail> GetUserProfilePercentage(int id);

    Task<int> UpdatePassword(t_UpdatePassword updatePassword);

    Task<int> UpdatePersonalDetail(t_user user);

    Task<t_user> GetUserProfile(int id);
}