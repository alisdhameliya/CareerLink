using Npgsql;

public class UserSkillsRepository : IUserSkills
{
    private readonly NpgsqlConnection _conn;
    public UserSkillsRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> AddUserSkills(t_UserSkills skills)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "insert into t_user_skills (c_skill_name,c_user_id) values (@c_skill_name,@c_user_id)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_skill_name", skills.c_skill_name);
            cmd.Parameters.AddWithValue("@c_user_id", skills.c_user_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while adding user skills:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }



    public async Task<List<t_UserSkills>> GetUserSkills(int id)
    {
        List<t_UserSkills> tt = new List<t_UserSkills>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_user_skills where c_user_id=@c_user_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_UserSkills
                    {
                        c_skill_id = Convert.ToInt32(dr["c_skill_id"]),
                        c_skill_name = dr["c_skill_name"].ToString(),
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user skills" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> UpdateUserSkills(t_UserSkills t_UserSkills)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_user_skills set c_skill_name=@c_skill_name where c_skill_id=@c_skill_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_skill_id", t_UserSkills.c_skill_id);
            cmd.Parameters.AddWithValue("@c_skill_name", t_UserSkills.c_skill_name);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating user skills" + ex.Message);
            return 0;
        }
    }

    public async Task<int> DeleteUserSkills(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "delete from t_user_skills where c_skill_id=@c_skill_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_skill_id", id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while deleting user skills:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<t_UserSkills> GetOneSkill(int id)
    {
        t_UserSkills tt = new t_UserSkills();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_user_skills where c_skill_id=@c_skill_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_skill_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt = new t_UserSkills();
                    tt.c_skill_id = Convert.ToInt32(dr["c_skill_id"]);
                    tt.c_skill_name = dr["c_skill_name"].ToString();
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user skills" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }
}