using System.Data.Common;
using Npgsql;

public class UserProfileDetail : IUserProfileDetail
{
    private readonly NpgsqlConnection _conn;
    public UserProfileDetail(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<t_user> GetUserProfile(int id)
    {
        t_user tu = null;
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_user where c_uid=@c_uid";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_uid",id);
            var dr = await cmd.ExecuteReaderAsync();
            if(dr.HasRows){
                while (await dr.ReadAsync())
                {
                    tu = new t_user();
                    tu.c_email = dr["c_email"].ToString();
                    tu.c_fullName = dr["c_fullName"].ToString();
                    tu.c_gender = dr["c_gender"].ToString();
                    tu.c_phoneNumber = dr["c_phone_number"].ToString();
                    tu.c_profileImage = dr["c_image"].ToString();
                    tu.c_username = dr["c_username"].ToString();
                    tu.c_gender = dr["c_gender"].ToString();
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while fetching the user profile data:"+ex.Message);
        }
        finally{
            await _conn.CloseAsync();
        }
        return tu;
    }

    // public async Task<t_UserProfileDetail> GetUserProfileDetail(int id)
    // {
    //     t_UserProfileDetail tt = null;
    //     try
    //     {
    //         await _conn.OpenAsync();
    //        string qry = @"
    // SELECT 
    //     u.c_uid, u.c_fullname, u.c_email, u.c_phone_number, u.c_gender, u.c_image, u.c_role,
    //     e.c_education_id, e.c_Degree, e.c_HighestQualification, e.c_percentage, 
    //     e.c_Specialization, e.c_UniversityName, e.c_YearOfPassing,
    //     ex.c_CompanyName, ex.c_JobTitle, ex.c_JobDesc, ex.c_years_work, ex.c_CurrentlyWorking,
    //     uk.c_skill_name,
    //     ujp.c_PreferredLocations, ujp.c_PreferredRoles, ujp.c_PreferredSalary,
    //     up.c_Project_Title, up.c_Project_Description, up.c_ProjectLink, up.c_TechnologiesUsed
    // FROM t_user u
    // LEFT JOIN t_education e ON u.c_uid = e.c_user_id
    // LEFT JOIN t_workexperience ex ON u.c_uid = ex.c_user_id
    // LEFT JOIN t_user_skills uk ON u.c_uid = uk.c_user_id
    // LEFT JOIN t_usercertifications uc ON u.c_uid = uc.c_user_id
    // LEFT JOIN t_userprojects up ON u.c_uid = up.c_user_id
    // LEFT JOIN t_userjobpreferences ujp ON u.c_uid = ujp.c_user_id
    // LEFT JOIN t_userresumes ur ON u.c_uid = ur.c_user_id
    // WHERE u.c_uid = @c_uid;";
    //         var cmd = new NpgsqlCommand(qry,_conn);
    //         cmd.Parameters.AddWithValue("@c_uid",id);
    //         var dr = await cmd.ExecuteReaderAsync();
    //         if(dr.HasRows){
    //             while (await dr.ReadAsync())
    //             {
    //                 tt = new t_UserProfileDetail{
    //                     c_user_id = Convert.ToInt32(dr["c_uid"]),
    //                     c_fullname = dr["c_fullname"].ToString(),
    //                     c_email = dr["c_email"].ToString(),
    //                     c_mobile_number = dr["c_phone_number"].ToString(),
    //                     c_gender = dr["c_gender"].ToString(),
    //                     c_image = dr["c_image"].ToString(),
    //                     c_role = dr["c_role"].ToString(),
    //                     education_Details = new t_Education_Details{
    //                         c_education_id = Convert.ToInt32(dr["c_education_id"]),
    //                         c_Degree = dr["c_Degree"].ToString(),
    //                         c_HighestQualification = dr["c_HighestQualification"].ToString(),
    //                         c_percentage = Convert.ToInt32(dr["c_percentage"]),
    //                         c_Specialization = dr["c_Specialization"].ToString(),
    //                         c_UniversityName = dr["c_UniversityName"].ToString(),
    //                         c_YearOfPassing = Convert.ToInt32(dr["c_YearOfPassing"]),
    //                     },
    //                     experience = new t_Work_Experience{
    //                         c_CompanyName = dr["c_CompanyName"].ToString(),
    //                         c_JobTitle = dr["c_JobTitle"].ToString(),
    //                         c_JobDesc = dr["c_JobDesc"].ToString(),
    //                         c_years_work = Convert.ToInt32(dr["c_years_work"]),
    //                         c_CurrentlyWorking = (bool)dr["c_CurrentlyWorking"],
    //                     },
    //                     skills = new t_UserSkills{
    //                         c_skill_name = dr["c_skill_name"].ToString(),
    //                     },
    //                     preference = new t_JobPreference{
    //                         c_PreferredLocations = dr["c_PreferredLocations"].ToString(),
    //                         c_PreferredRoles = dr["c_PreferredRoles"].ToString(),
    //                         c_PreferredSalary = dr["c_PreferredSalary"].ToString(),
    //                     },
    //                     projects = new t_UserProjects{
    //                         c_Project_Title = dr["c_Project_Title"].ToString(),
    //                         c_Project_Description = dr["c_Project_Description"].ToString(),
    //                         c_ProjectLink = dr["c_ProjectLink"].ToString(),
    //                         c_TechnologiesUsed = dr["c_TechnologiesUsed"].ToString()
    //                     }
    //                 };
    //             }
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         System.Console.WriteLine("Error while getting user profile details:"+ex.Message);            
    //     }
    //     finally{
    //         await _conn.CloseAsync();
    //     }
    //     return tt;
    // }

    public async Task<t_UserProfileDetail> GetUserProfileDetail(int id)
    {
        t_UserProfileDetail tt = null;
        try
        {
            await _conn.OpenAsync();
            string qry = @"
    SELECT 
        u.c_uid, u.c_fullname, u.c_email, u.c_phone_number, u.c_gender, u.c_image, u.c_role,
        e.c_education_id, e.c_Degree, e.c_HighestQualification, e.c_percentage, 
        e.c_Specialization, e.c_UniversityName, e.c_YearOfPassing,
        ex.c_CompanyName, ex.c_JobTitle, ex.c_JobDesc, ex.c_years_work, ex.c_CurrentlyWorking,
        uk.c_skill_name,
        ujp.c_PreferredLocations, ujp.c_PreferredRoles, ujp.c_PreferredSalary,
        up.c_Project_Title, up.c_Project_Description, up.c_ProjectLink, up.c_TechnologiesUsed
    FROM t_user u
    LEFT JOIN t_education e ON u.c_uid = e.c_user_id
    LEFT JOIN t_workexperience ex ON u.c_uid = ex.c_user_id
    LEFT JOIN t_user_skills uk ON u.c_uid = uk.c_user_id
    LEFT JOIN t_usercertifications uc ON u.c_uid = uc.c_user_id
    LEFT JOIN t_userprojects up ON u.c_uid = up.c_user_id
    LEFT JOIN t_userjobpreferences ujp ON u.c_uid = ujp.c_user_id
    LEFT JOIN t_userresumes ur ON u.c_uid = ur.c_user_id
    WHERE u.c_uid = @c_uid;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_uid", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt = new t_UserProfileDetail
                    {
                        c_user_id = Convert.ToInt32(dr["c_uid"]),
                        c_fullname = dr["c_fullname"].ToString(),
                        c_email = dr["c_email"].ToString(),
                        c_mobile_number = dr["c_phone_number"].ToString(),
                        c_gender = dr["c_gender"].ToString(),
                        c_image = dr["c_image"].ToString(),
                        c_role = dr["c_role"].ToString(),
                        education_Details = new t_Education_Details
                        {
                            c_education_id = Convert.ToInt32(dr["c_education_id"]),
                            c_Degree = dr["c_Degree"].ToString(),
                            c_HighestQualification = dr["c_HighestQualification"].ToString(),
                            c_percentage = Convert.ToInt32(dr["c_percentage"]),
                            c_Specialization = dr["c_Specialization"].ToString(),
                            c_UniversityName = dr["c_UniversityName"].ToString(),
                            c_YearOfPassing = Convert.ToInt32(dr["c_YearOfPassing"]),
                        },
                        experience = new t_Work_Experience
                        {
                            c_CompanyName = dr["c_CompanyName"].ToString(),
                            c_JobTitle = dr["c_JobTitle"].ToString(),
                            c_JobDesc = dr["c_JobDesc"].ToString(),
                            c_years_work = Convert.ToInt32(dr["c_years_work"]),
                            c_CurrentlyWorking = (bool)dr["c_CurrentlyWorking"],
                        },
                        skills = new t_UserSkills
                        {
                            c_skill_name = dr["c_skill_name"].ToString(),
                        },
                        preference = new t_JobPreference
                        {
                            c_PreferredLocations = dr["c_PreferredLocations"].ToString(),
                            c_PreferredRoles = dr["c_PreferredRoles"].ToString(),
                            c_PreferredSalary = dr["c_PreferredSalary"].ToString(),
                        },
                        projects = new t_UserProjects
                        {
                            c_Project_Title = dr["c_Project_Title"].ToString(),
                            c_Project_Description = dr["c_Project_Description"].ToString(),
                            c_ProjectLink = dr["c_ProjectLink"].ToString(),
                            c_TechnologiesUsed = dr["c_TechnologiesUsed"].ToString()
                        }
                    };
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user profile details:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<t_UserProfileDetail> GetUserProfilePercentage(int id)
    {
        t_UserProfileDetail tt = null;
        try
        {
            await _conn.OpenAsync();
            string qry = @"SELECT 
                ROUND(( 
                    (CASE WHEN EXISTS (SELECT 1 FROM t_Education WHERE c_user_id = @c_user_id) THEN 1 ELSE 0 END) +
                    (CASE WHEN EXISTS (SELECT 1 FROM t_WorkExperience WHERE c_user_id = @c_user_id) THEN 1 ELSE 0 END) +
                    (CASE WHEN EXISTS (SELECT 1 FROM t_user_skills WHERE c_user_id = @c_user_id) THEN 1 ELSE 0 END) +
                    (CASE WHEN EXISTS (SELECT 1 FROM t_UserCertifications WHERE c_user_id = @c_user_id) THEN 1 ELSE 0 END) +
                    (CASE WHEN EXISTS (SELECT 1 FROM t_UserProjects WHERE c_user_id = @c_user_id) THEN 1 ELSE 0 END) +
                    (CASE WHEN EXISTS (SELECT 1 FROM t_UserResumes WHERE c_user_id = @c_user_id AND c_ResumeFilePath IS NOT NULL) THEN 1 ELSE 0 END) +
                    (CASE WHEN EXISTS (SELECT 1 FROM t_UserJobPreferences WHERE c_user_id = @c_user_id) THEN 1 ELSE 0 END)
                ) / 7.0 * 100, 0) AS profile_completion_percentage;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt = new t_UserProfileDetail();
                    tt.profile_completion_percentage = Convert.ToInt32(dr["profile_completion_percentage"]);
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user profle in percentage:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> UpdatePassword(t_UpdatePassword updatePassword)
    {
        try
        {
            // Step 1: Get the current hashed password from the database
            const string fetchQuery = "SELECT c_password FROM t_user WHERE c_uid = @c_user_id";
            await using var fetchCmd = new NpgsqlCommand(fetchQuery, _conn);
            fetchCmd.Parameters.AddWithValue("@c_user_id", updatePassword.c_user_id);

            await _conn.OpenAsync();
            object? result = await fetchCmd.ExecuteScalarAsync();
            await _conn.CloseAsync(); // Close after fetching

            if (result == null)
            {
                return -1; // User not found
            }

            string currentHashedPassword = result.ToString()!;

            // Step 2: Verify old password using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(updatePassword.c_OldPassword, currentHashedPassword))
            {
                return -1; // Old password is incorrect
            }

            // Step 3: Hash new password with BCrypt
            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(updatePassword.c_NewPassword);

            // Step 4: Update password in database
            const string updateQuery = "UPDATE t_user SET c_password = @c_NewPassword WHERE c_uid = @c_user_id";
            await using var updateCmd = new NpgsqlCommand(updateQuery, _conn);
            updateCmd.Parameters.AddWithValue("@c_user_id", updatePassword.c_user_id);
            updateCmd.Parameters.AddWithValue("@c_NewPassword", newHashedPassword); // Store hashed password

            await _conn.OpenAsync();
            int rowsAffected = await updateCmd.ExecuteNonQueryAsync();
            await _conn.CloseAsync();

            return rowsAffected > 0 ? 1 : 0; // Return 1 if update succeeded, 0 otherwise
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdatePassword: {ex.Message}");
            return 0;
        }
    }

    public async Task<int> UpdatePersonalDetail(t_user user)
    {
        try
        {
            await _conn.OpenAsync();
            const string query = @"UPDATE t_user SET c_fullname = @c_fullname, c_phone_number = @PhoneNumber, c_gender = @Gender, c_image = @Image WHERE c_uid = @c_uid";

            await using var cmd = new NpgsqlCommand(query, _conn);
            cmd.Parameters.AddWithValue("@c_fullname", user.c_fullName ??(object)DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", user.c_phoneNumber??(object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Gender", user.c_gender??(object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Image", (object?)user.c_profileImage ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@c_uid",user.c_userId);

            int result = await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateProfile: {ex.Message}");
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }
}