using Npgsql;

public class UserCertificateRepository:IUserCertificate
{
    private readonly NpgsqlConnection _conn;
    public UserCertificateRepository(NpgsqlConnection conn){
        _conn = conn;
    }

    public async Task<int> AddUserCertificate(t_User_Certificate certificate)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "insert into t_UserCertifications (c_user_id,c_CertificateFilePath) values (@c_user_id,@c_CertificateFilePath)";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_user_id",certificate.c_user_id);
            cmd.Parameters.AddWithValue("@c_CertificateFilePath",certificate.c_CertificateFilePath);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while adding user certificate:"+ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }

    public async Task<int> DeleteUserCertificate(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "delete from t_usercertifications where c_CertificationID=@c_CertificationID";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_CertificationID",id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while deleting user certificate:"+ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }

    public async Task<List<t_User_Certificate>> GetAllUserCertificate(int id)
    {
        List<t_User_Certificate> tt = new List<t_User_Certificate>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_UserCertifications where c_user_id=@c_user_id";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_user_id",id);
            var dr = await cmd.ExecuteReaderAsync();
            if(dr.HasRows){
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_User_Certificate{
                        c_CertificationID = Convert.ToInt32(dr["c_CertificationID"]),
                        c_CertificateFilePath = dr["c_CertificateFilePath"].ToString()
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user certificates:"+ ex.Message);
        }
        finally{
            await _conn.CloseAsync();
        }
        return tt;
    }

   

    public async Task<int> UpdateUserCertificate(t_User_Certificate certificate)
    {
        try
        {
            await _conn.OpenAsync();
             string qry = "update t_UserCertifications set c_CertificateFilePath=@c_CertificateFilePath where c_CertificationID=@c_CertificationID";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_CertificateFilePath",certificate.c_CertificateFilePath);
            cmd.Parameters.AddWithValue("@c_CertificationID",certificate.c_CertificationID);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating user certificate:" + ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }

     public async Task<t_User_Certificate> GetOneCertificate(int id)
    {
        t_User_Certificate tt = new t_User_Certificate();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_UserCertifications where c_CertificationID=@c_CertificationID";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_CertificationID",id);
            var dr = await cmd.ExecuteReaderAsync();
            if(dr.HasRows){
                while (await dr.ReadAsync())
                {
                   tt = new t_User_Certificate();
                   tt.c_CertificationID = Convert.ToInt32(dr["c_CertificationID"]);
                  tt.c_CertificateFilePath = dr["c_CertificateFilePath"].ToString();
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user certificates:"+ ex.Message);
        }
        finally{
            await _conn.CloseAsync();
        }
        return tt;
    }
}