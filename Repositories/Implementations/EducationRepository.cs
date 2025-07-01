using System.Data.Common;
using Npgsql;

public class EducationRepository : IEducation_Details
{
    private readonly NpgsqlConnection _conn;
    public EducationRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> AddEducationDetails(t_Education_Details t_Education)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "insert into t_Education (c_user_id,c_HighestQualification,c_Degree,c_Specialization,c_UniversityName,c_YearOfPassing,c_percentage) values (@c_user_id,@c_HighestQualification,@c_Degree,@c_Specialization,@c_UniversityName,@c_YearOfPassing,@c_percentage)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", t_Education.c_user_id);
            cmd.Parameters.AddWithValue("@c_HighestQualification", t_Education.c_HighestQualification);
            cmd.Parameters.AddWithValue("@c_Degree", t_Education.c_Degree ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@c_Specialization", t_Education.c_Specialization ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@c_UniversityName", t_Education.c_UniversityName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@c_YearOfPassing", t_Education.c_YearOfPassing);
            cmd.Parameters.AddWithValue("@c_percentage", t_Education.c_percentage);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while adding education detials:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> DeleteEducationDetails(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "delete from t_Education where c_education_id=@c_education_id";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_education_id",id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while deleting education details:"+ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }

    public async Task<List<t_Education_Details>> GetEducation_Details(int id)
    {
        List<t_Education_Details> td = new List<t_Education_Details>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_education where c_user_id=@c_user_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id",id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    td.Add(new t_Education_Details
                    {
                        c_education_id = dr["c_education_id"] != DBNull.Value ? Convert.ToInt32(dr["c_education_id"]) : 0,
                        c_HighestQualification = dr["c_HighestQualification"] != DBNull.Value ? dr["c_HighestQualification"].ToString() : string.Empty,
                        c_Degree = dr["c_Degree"] != DBNull.Value ? dr["c_Degree"].ToString() : string.Empty,
                        c_Specialization = dr["c_Specialization"] != DBNull.Value ? dr["c_Specialization"].ToString() : string.Empty,
                        c_UniversityName = dr["c_UniversityName"] != DBNull.Value ? dr["c_UniversityName"].ToString() : string.Empty,
                        c_YearOfPassing = dr["c_YearOfPassing"] != DBNull.Value ? Convert.ToInt32(dr["c_YearOfPassing"]) : 0,
                        c_percentage = dr["c_percentage"] != DBNull.Value ? Convert.ToDecimal(dr["c_percentage"]) : 0m
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting education detials for user:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return td;
    }

    

    public async Task<int> UpdateEducationDetails(t_Education_Details t_Education)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_education set c_HighestQualification=@c_HighestQualification,c_Degree=@c_Degree,c_Specialization=@c_Specialization,c_UniversityName=@c_UniversityName,c_YearOfPassing=@c_YearOfPassing,c_percentage=@c_percentage where c_education_id=@c_education_id";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_HighestQualification",t_Education.c_HighestQualification);
            cmd.Parameters.AddWithValue("@c_Degree",t_Education.c_Degree?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@c_Specialization",t_Education.c_Specialization?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@c_UniversityName",t_Education.c_UniversityName?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@c_YearOfPassing",t_Education.c_YearOfPassing );
            cmd.Parameters.AddWithValue("@c_percentage",t_Education.c_percentage);
            cmd.Parameters.AddWithValue("@c_education_id",t_Education.c_education_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updaing user education details"+ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }

    public async Task<t_Education_Details> GetOneEducation(int id)
    {
        t_Education_Details tt = null;
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_Education where c_education_id=@c_education_id";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_education_id",id);
            var dr = await cmd.ExecuteReaderAsync();
            if(dr.HasRows){
                while (await dr.ReadAsync())
                {
                    tt = new t_Education_Details();
                    tt.c_education_id = Convert.ToInt32(dr["c_education_id"]);
                    tt.c_HighestQualification = dr["c_HighestQualification"].ToString();
                    tt.c_Degree = dr["c_Degree"].ToString();
                    tt.c_Specialization  = dr["c_Specialization"].ToString();
                    tt.c_UniversityName = dr["c_UniversityName"].ToString();
                    tt.c_YearOfPassing = Convert.ToInt32(dr["c_YearOfPassing"]);
                    tt.c_percentage = Convert.ToInt32(dr["c_percentage"]);
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting one education detials:"+ex.Message);            
        }
        finally{
            await _conn.CloseAsync();
        }
        return tt;
    }
}