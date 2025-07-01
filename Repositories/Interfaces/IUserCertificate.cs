public interface IUserCertificate
{
    Task<int> AddUserCertificate(t_User_Certificate certificate);
    Task<List<t_User_Certificate>> GetAllUserCertificate(int id);
    Task<int> UpdateUserCertificate(t_User_Certificate certificate);

    Task<int> DeleteUserCertificate(int id);

    Task<t_User_Certificate> GetOneCertificate(int id);
}