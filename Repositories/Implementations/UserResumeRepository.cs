using Npgsql;

public class UserResumeRepository : IUserResume
{
    private readonly NpgsqlConnection _conn;
    public UserResumeRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    // public async Task<int> AddUserResume(t_UserResume resume)
    // {
    //     try
    //     {
    //         await _conn.OpenAsync();
    //         string qry = "insert into t_UserResumes (c_user_id,c_ResumeFilePath) values (@c_user_id,@c_ResumeFilePath)";
    //         var cmd = new NpgsqlCommand(qry,_conn);
    //         cmd.Parameters.AddWithValue("@c_ResumeFilePath",resume.c_ResumeFilePath);
    //         cmd.Parameters.AddWithValue("@c_user_id",resume.c_user_id);
    //         await cmd.ExecuteNonQueryAsync();
    //         return 1;
    //     }
    //     catch (System.Exception ex)
    //     {
    //         System.Console.WriteLine("Error while adding user resume"+ex.Message);
    //         return 0;
    //     }
    //     finally{
    //         await _conn.CloseAsync();
    //     }
    // }

    public async Task<int> AddUserResume(t_UserResume resume)
    {
        try
        {
            await _conn.OpenAsync();

            // Check if a resume already exists for the given user
            string checkQry = "SELECT COUNT(*) FROM t_UserResumes WHERE c_user_id = @c_user_id";
            using (var checkCmd = new NpgsqlCommand(checkQry, _conn))
            {
                checkCmd.Parameters.AddWithValue("@c_user_id", resume.c_user_id);
                int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                if (count > 0)
                {
                    // Resume already exists
                    return -1;  // Indicate duplicate entry
                }
            }

            // Insert new resume if not already present
            string qry = "INSERT INTO t_UserResumes (c_user_id, c_ResumeFilePath) VALUES (@c_user_id, @c_ResumeFilePath)";
            using (var cmd = new NpgsqlCommand(qry, _conn))
            {
                cmd.Parameters.AddWithValue("@c_ResumeFilePath", resume.c_ResumeFilePath);
                cmd.Parameters.AddWithValue("@c_user_id", resume.c_user_id);
                await cmd.ExecuteNonQueryAsync();
            }

            return 1; // Successfully inserted
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while adding user resume: " + ex.Message);
            return 0; // Error occurred
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }


    public async Task<int> UpdateUserResume(t_UserResume resume)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_UserResumes set c_ResumeFilePath=@c_ResumeFilePath where c_ResumeID=@c_ResumeID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_ResumeID", resume.c_ResumeID);
            cmd.Parameters.AddWithValue("@c_ResumeFilePath", resume.c_ResumeFilePath);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating user resume:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> DeleteUserResume(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "delete from t_userresumes where c_ResumeID=@c_ResumeID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_ResumeID", id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while deleting user resume:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<t_UserResume> GetUserResume(int id)
    {
        t_UserResume tr = null;
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_userresumes where c_user_id=@c_user_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tr = new t_UserResume();
                    tr.c_ResumeID = Convert.ToInt32(dr["c_ResumeID"]);
                    tr.c_ResumeFilePath = dr["c_ResumeFilePath"].ToString();
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user resume:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tr;
    }

    public async Task<t_UserResume> GetOneResume(int id)
    {
        t_UserResume tr = null;
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_userresumes where c_ResumeID=@c_ResumeID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_ResumeID", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tr = new t_UserResume();
                    tr.c_ResumeID = Convert.ToInt32(dr["c_ResumeID"]);
                    tr.c_ResumeFilePath = dr["c_ResumeFilePath"].ToString();
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user resume:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tr;
    }
}