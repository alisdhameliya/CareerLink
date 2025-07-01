using Npgsql;

public class WorkExperienceRepository : IWorkExperience
{
    private readonly NpgsqlConnection _conn;
    public WorkExperienceRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> AddWorkExperience(t_Work_Experience experience)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "insert into t_WorkExperience (c_user_id,c_CompanyName,c_JobTitle,c_JobDesc,c_years_work,c_CurrentlyWorking) values (@c_user_id,@c_CompanyName,@c_JobTitle,@c_JobDesc,@c_years_work,@c_CurrentlyWorking)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", experience.c_user_id);
            cmd.Parameters.AddWithValue("@c_CompanyName", experience.c_CompanyName);
            cmd.Parameters.AddWithValue("@c_JobTitle", experience.c_JobTitle);
            cmd.Parameters.AddWithValue("@c_JobDesc", experience.c_JobDesc);
            cmd.Parameters.AddWithValue("@c_years_work", experience.c_years_work);
            cmd.Parameters.AddWithValue("@c_CurrentlyWorking", experience.c_CurrentlyWorking);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while adding work experience:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }


    public async Task<List<t_Work_Experience>> GetWorkExperience(int id)
    {
        List<t_Work_Experience> tt = new List<t_Work_Experience>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_WorkExperience where c_user_id=@c_user_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_Work_Experience
                    {
                        c_WorkID = Convert.ToInt32(dr["c_workid"]),
                        c_CompanyName = dr["c_CompanyName"].ToString(),
                        c_JobTitle = dr["c_JobTitle"].ToString(),
                        c_JobDesc = dr["c_JobDesc"].ToString(),
                        c_years_work = Convert.ToInt32(dr["c_years_work"]),
                        c_CurrentlyWorking = (bool)dr["c_CurrentlyWorking"],
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting work experience" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> UpdateWorkExperience(t_Work_Experience experience)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_WorkExperience set c_CompanyName=@c_CompanyName,c_JobTitle=@c_JobTitle,c_JobDesc=@c_JobDesc,c_years_work=@c_years_work,c_CurrentlyWorking=@c_CurrentlyWorking where c_WorkID=@c_WorkID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_WorkID", experience.c_WorkID);
            cmd.Parameters.AddWithValue("@c_CompanyName", experience.c_CompanyName);
            cmd.Parameters.AddWithValue("@c_JobTitle", experience.c_JobTitle);
            cmd.Parameters.AddWithValue("@c_JobDesc", experience.c_JobDesc);
            cmd.Parameters.AddWithValue("@c_years_work", experience.c_years_work);
            cmd.Parameters.AddWithValue("@c_CurrentlyWorking", experience.c_CurrentlyWorking);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while upating work experience:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
    public async Task<int> DeleteWorkExperience(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "delete from t_WorkExperience where c_WorkID=@c_WorkID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_WorkID", id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while deleting work experience: " + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<t_Work_Experience> GetOneWorkExperience(int id)
    {
        t_Work_Experience tt = new t_Work_Experience();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_WorkExperience where c_workid=@c_workid";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_workid", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt = new t_Work_Experience();
                    tt.c_WorkID = Convert.ToInt32(dr["c_workid"]);
                    tt.c_CompanyName = dr["c_CompanyName"].ToString();
                    tt.c_JobTitle = dr["c_JobTitle"].ToString();
                    tt.c_JobDesc = dr["c_JobDesc"].ToString();
                    tt.c_years_work = Convert.ToInt32(dr["c_years_work"]);
                    tt.c_CurrentlyWorking = (bool)dr["c_CurrentlyWorking"];
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting work experience" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }
}