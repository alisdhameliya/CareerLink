using System.Linq.Expressions;
using System.Security.AccessControl;
using Npgsql;

public class UserProjectsRepository : IUserProjects
{
    private readonly NpgsqlConnection _conn;
    public UserProjectsRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> AddUserProjects(t_UserProjects projects)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "insert into t_UserProjects (c_user_id,c_Project_Title,c_Project_Description,c_TechnologiesUsed,c_ProjectLink) values (@c_user_id,@c_Project_Title,@c_Project_Description,@c_TechnologiesUsed,@c_ProjectLink)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", projects.c_user_id);
            cmd.Parameters.AddWithValue("@c_Project_Title", projects.c_Project_Title);
            cmd.Parameters.AddWithValue("@c_Project_Description", projects.c_Project_Description);
            cmd.Parameters.AddWithValue("@c_TechnologiesUsed", projects.c_TechnologiesUsed);
            cmd.Parameters.AddWithValue("@c_ProjectLink", projects.c_ProjectLink);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while adding user projects:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> DeleteUserProjects(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "delete from t_userprojects where c_ProjectID=@c_ProjectID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_ProjectID", id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while deleting user projects" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<t_UserProjects> GetOneProject(int id)
    {
        t_UserProjects tt = new t_UserProjects();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_UserProjects where c_ProjectID=@c_ProjectID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_ProjectID", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt = new t_UserProjects();
                    tt.c_ProjectID = Convert.ToInt32(dr["c_ProjectID"]);
                    tt.c_Project_Title = dr["c_Project_Title"].ToString();
                    tt.c_Project_Description = dr["c_Project_Description"].ToString();
                    tt.c_TechnologiesUsed = dr["c_TechnologiesUsed"].ToString();
                    tt.c_ProjectLink = dr["c_ProjectLink"].ToString();
                }
            }
        }
        catch (System.Exception ex)
        {

            System.Console.WriteLine("Error while getting one user projects:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<List<t_UserProjects>> GetUserProjects(int id)
    {
        List<t_UserProjects> tt = new List<t_UserProjects>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_UserProjects where c_user_id=@c_user_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_UserProjects
                    {
                        c_ProjectID = Convert.ToInt32(dr["c_ProjectID"]),
                        c_Project_Title = dr["c_Project_Title"].ToString(),
                        c_Project_Description = dr["c_Project_Description"].ToString(),
                        c_TechnologiesUsed = dr["c_TechnologiesUsed"].ToString(),
                        c_ProjectLink = dr["c_ProjectLink"].ToString(),
                    });
                }
            }
        }
        catch (System.Exception ex)
        {

            System.Console.WriteLine("Error while getting user projects:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> UpdateUserProjects(t_UserProjects projects)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_UserProjects set c_Project_Title=@c_Project_Title,c_Project_Description=@c_Project_Description,c_TechnologiesUsed=@c_TechnologiesUsed,c_ProjectLink=@c_ProjectLink where c_ProjectID=@c_ProjectID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_ProjectID", projects.c_ProjectID);
            cmd.Parameters.AddWithValue("@c_Project_Title", projects.c_Project_Title);
            cmd.Parameters.AddWithValue("@c_Project_Description", projects.c_Project_Description);
            cmd.Parameters.AddWithValue("@c_TechnologiesUsed", projects.c_TechnologiesUsed);
            cmd.Parameters.AddWithValue("@c_ProjectLink", projects.c_ProjectLink);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating user projects" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}