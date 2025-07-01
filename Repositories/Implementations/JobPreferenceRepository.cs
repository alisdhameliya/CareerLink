using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using Npgsql;
using NpgsqlTypes;

public class JobPreferenceRepository : IJobPreference
{
    private readonly NpgsqlConnection _conn;
    public JobPreferenceRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> AddJobPreference(t_JobPreference preference)
    {
        try
        {
            await _conn.OpenAsync();

            // Check if user already has a job preference record
            string checkQuery = "SELECT COUNT(*) FROM t_UserJobPreferences WHERE c_user_id = @c_user_id";
            var checkCmd = new NpgsqlCommand(checkQuery, _conn);
            checkCmd.Parameters.AddWithValue("@c_user_id", preference.c_user_id);

            int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

            if (count > 0)
            {
                Console.WriteLine("User already has a job preference record.");
                return -1; // Indicate duplicate entry attempt
            }

            // If no record exists, insert the new preference
            string insertQuery = @"
            INSERT INTO t_UserJobPreferences (c_user_id, c_PreferredRoles, c_PreferredSalary, c_PreferredLocations) 
            VALUES (@c_user_id, @c_PreferredRoles, @c_PreferredSalary, @c_PreferredLocations)";

            var cmd = new NpgsqlCommand(insertQuery, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", preference.c_user_id);
            cmd.Parameters.AddWithValue("@c_PreferredSalary", preference.c_PreferredSalary);
            cmd.Parameters.AddWithValue("@c_PreferredLocations", preference.c_PreferredLocations);
            cmd.Parameters.AddWithValue("@c_PreferredRoles", preference.c_PreferredRoles);
            await cmd.ExecuteNonQueryAsync();
            return 1; // Successfully inserted
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while adding job preference: " + ex.Message);
            return 0; // Error occurred
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> DeleteJobPreference(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "delete from t_userjobpreferences where c_PreferenceID=@c_PreferenceID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_PreferenceID", id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while deleting job preference:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<t_JobPreference> GetJobPreference(int id)
    {
        t_JobPreference jp = null;
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_userjobpreferences where c_user_id=@c_user_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    jp = new t_JobPreference();
                    jp.c_PreferenceID = Convert.ToInt32(dr["c_PreferenceID"]);
                    jp.c_PreferredRoles = dr["c_PreferredRoles"].ToString();
                    jp.c_PreferredSalary = dr["c_PreferredSalary"].ToString();
                    jp.c_PreferredLocations = dr["c_PreferredLocations"].ToString();
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user job preference" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return jp;
    }

    public async Task<int> UpdateJobPreference(t_JobPreference preference)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_userjobpreferences set c_PreferredRoles=@c_PreferredRoles,c_PreferredSalary=@c_PreferredSalary,c_PreferredLocations=@c_PreferredLocations where c_PreferenceID=@c_PreferenceID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_PreferenceID", preference.c_PreferenceID);
             cmd.Parameters.AddWithValue("@c_user_id", preference.c_user_id);
            cmd.Parameters.AddWithValue("@c_PreferredSalary", preference.c_PreferredSalary);
            cmd.Parameters.AddWithValue("@c_PreferredLocations", preference.c_PreferredLocations);
            cmd.Parameters.AddWithValue("@c_PreferredRoles", preference.c_PreferredRoles);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating job preference:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<t_JobPreference> GetOneJobPreference(int id)
    {
        t_JobPreference tt = null;
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_UserJobPreferences where c_PreferenceID=@c_PreferenceID";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_PreferenceID", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt = new t_JobPreference();
                    tt.c_PreferenceID = Convert.ToInt32(dr["c_PreferenceID"]);
                    tt.c_PreferredRoles = dr["c_PreferredRoles"].ToString();
                    tt.c_PreferredSalary = dr["PreferredSalary"].ToString();
                    tt.c_PreferredLocations = dr["PreferredLocations"].ToString();
                }
            }
        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }
}