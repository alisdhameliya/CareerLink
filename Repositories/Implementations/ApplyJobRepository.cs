using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Npgsql;
using Repositories.Model;

public class ApplyJobRepository : IApplyjobInterface
{
    private readonly NpgsqlConnection _conn;
    public ApplyJobRepository(NpgsqlConnection conn)
    {
        _conn = conn;
    }

    public async Task<int> ApplyJob(t_apply_job apply_Job)
    {
        try
        {
            await _conn.OpenAsync();

            // First: Check if already applied
            string checkqry = "SELECT 1 FROM t_apply_jobs WHERE c_uid = @c_uid AND c_job_id = @c_job_id";
            var checkcmd = new NpgsqlCommand(checkqry, _conn);
            checkcmd.Parameters.AddWithValue("@c_uid", apply_Job.c_uid);
            checkcmd.Parameters.AddWithValue("@c_job_id", apply_Job.c_job_id);

            var dr = await checkcmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                await dr.CloseAsync();  // ✅ close reader before returning
                return 2; // Already applied
            }
            await dr.CloseAsync(); // ✅ close reader to avoid conflict

            apply_Job.c_status = "Pending";
            apply_Job.c_apply_date = DateTime.Now.ToString("yyyy-MM-dd");

            // Insert if not applied
            string qry = "INSERT INTO t_apply_jobs (c_job_id, c_uid, c_apply_date, c_status) VALUES (@c_job_id, @c_uid, @c_apply_date, @c_status)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_job_id", apply_Job.c_job_id);
            cmd.Parameters.AddWithValue("@c_uid", apply_Job.c_uid);
            cmd.Parameters.AddWithValue("@c_apply_date", apply_Job.c_apply_date);
            cmd.Parameters.AddWithValue("@c_status", apply_Job.c_status);

            await cmd.ExecuteNonQueryAsync();
            return 1; // Successfully applied
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while applying job: " + ex.Message);
            return 0; // Error
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }


    public async Task<FieldCheckResult?> CheckFields(int id)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = @"
            SELECT 
                CASE WHEN EXISTS (SELECT 1 FROM t_education e WHERE e.c_user_id = u.c_uid) THEN 1 ELSE 0 END AS HasEducation,
                CASE WHEN EXISTS (SELECT 1 FROM t_workexperience ex WHERE ex.c_user_id = u.c_uid) THEN 1 ELSE 0 END AS HasExperience,
                CASE WHEN EXISTS (SELECT 1 FROM t_userresumes r WHERE r.c_user_id = u.c_uid) THEN 1 ELSE 0 END AS HasResume
            FROM t_user u
            WHERE u.c_uid = @c_uid";

            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_uid", id);

            using var dr = await cmd.ExecuteReaderAsync();
            if (await dr.ReadAsync())
            {
                return new FieldCheckResult
                {
                    HasEducation = dr.GetInt32(0) == 1,
                    HasExperience = dr.GetInt32(1) == 1,
                    HasResume = dr.GetInt32(2) == 1
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while checking fields: " + ex.Message);
            return null;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    // public async Task<int> DeleteJob(int id)
    // {
    //     try
    //     {
    //         await _conn.OpenAsync();
    //         string qry = "delete from t_apply_jobs where c_application_id=@c_application_id";
    //         var cmd = new NpgsqlCommand(qry, _conn);
    //         cmd.Parameters.AddWithValue("@c_application_id", id);
    //         await cmd.ExecuteNonQueryAsync();
    //         return 1;
    //     }
    //     catch (System.Exception ex)
    //     {

    //         System.Console.WriteLine("Error while deleting applied jobs:" + ex.Message);
    //         return 0;
    //     }
    //     finally
    //     {
    //         await _conn.CloseAsync();
    //     }
    // }


    //To get all job application, Also For filter on job title and also filter by status
    public async Task<List<t_apply_job>> GetApplyJobApplication(int id, string job_title, string status)
    {
        List<t_apply_job> tt = new List<t_apply_job>();
        try
        {
            await _conn.OpenAsync();
            string qry = @"SELECT 
                            aj.c_application_id,
                            aj.c_apply_date,
                            aj.c_status,
                            
                            u.c_uid,
                            u.c_fullname,
                            u.c_email,
                            u.c_phone_number,
                            u.c_image,

                            ed.c_highestqualification,
                            ed.c_degree,
                            ed.c_universityname,
                            ed.c_specialization,
                            ed.c_yearofpassing,
                            ed.c_board,
                            ed.c_percentage,

                            we.c_companyname,
                            we.c_jobtitle,
                            we.c_years_work,

                            r.c_resumefilepath,
                            aj.*,
                            t.c_job_title

                        FROM t_apply_jobs aj

                        INNER JOIN t_job_post t ON aj.c_job_id = t.c_job_id

                        INNER JOIN t_user u ON aj.c_uid = u.c_uid

                        LEFT JOIN t_education ed ON u.c_uid = ed.c_user_id

                        LEFT JOIN t_workexperience we ON u.c_uid = we.c_user_id

                        LEFT JOIN t_userresumes r ON u.c_uid = r.c_user_id

                        WHERE t.c_company_id = c_company_id
                         AND (@job_title IS NULL OR t.c_job_title ILIKE @job_title)
                         AND (@c_status IS NULL OR  aj.c_status ILIKE @c_status)
                        ORDER BY aj.c_apply_date DESC;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            // ↓ OR explicitly set type if it's null:
            if (string.IsNullOrEmpty(job_title))
            {
                cmd.Parameters.Add("@job_title", NpgsqlTypes.NpgsqlDbType.Varchar).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.AddWithValue("@job_title", job_title);
            }
            // ↓ OR explicitly set type if it's null:
            if (string.IsNullOrEmpty(status))
            {
                cmd.Parameters.Add("@c_status", NpgsqlTypes.NpgsqlDbType.Varchar).Value = DBNull.Value;
            }
            else
            {
                cmd.Parameters.AddWithValue("@c_status", status);
            }
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_apply_job
                    {
                        c_application_id = Convert.ToInt32(dr["c_application_id"]),
                        c_apply_date = dr["c_apply_date"].ToString(),
                        c_status = dr["c_status"].ToString(),
                        user = new t_user
                        {
                            c_fullName = dr["c_fullname"]?.ToString(),
                            c_email = dr["c_email"]?.ToString(),
                            c_phoneNumber = dr["c_phone_number"]?.ToString(),
                            c_profileImage = dr["c_image"].ToString(),
                        },
                        education_Details = new t_Education_Details
                        {
                            c_HighestQualification = dr["c_HighestQualification"]?.ToString(),
                            c_Degree = dr["c_Degree"]?.ToString(),
                            c_UniversityName = dr["c_UniversityName"]?.ToString(),
                            c_Specialization = dr["c_Specialization"]?.ToString(),
                            c_YearOfPassing = dr["c_YearOfPassing"] != DBNull.Value ? Convert.ToInt32(dr["c_YearOfPassing"]) : 0,
                        },
                        work_Experience = new t_Work_Experience
                        {
                            c_CompanyName = dr["c_CompanyName"]?.ToString(),
                            c_JobTitle = dr["c_JobTitle"]?.ToString(),
                            c_years_work = dr["c_years_work"] != DBNull.Value ? Convert.ToInt32(dr["c_years_work"]) : 0,
                        },
                        userResume = new t_UserResume
                        {
                            c_ResumeFilePath = dr["c_ResumeFilePath"]?.ToString(),
                        },
                        job_Post = new Job_Post{
                            c_job_title = dr["c_job_title"].ToString(),
                        }
                    });

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

    public async Task<int> UpdateStatusOfJobApplication(t_apply_job apply_Job)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "update t_apply_jobs set c_status=@c_status where c_application_id=@c_application_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_status", apply_Job.c_status);
            cmd.Parameters.AddWithValue("@c_application_id", apply_Job.c_application_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while update job application status:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<t_apply_job>> GetUserAppliedJobApplication(int id)
    {
        List<t_apply_job> tt = new List<t_apply_job>();
        try
        {
            await _conn.OpenAsync();
            string qry = @"SELECT 
                        aj.c_application_id,
                        aj.c_job_id,
                        jp.c_job_title AS JobTitle,
                        c.c_company_name AS CompanyName,
                        c.c_company_logo AS CompanyLogo,
                        jp.c_job_location AS Location
                    FROM t_apply_jobs aj
                    INNER JOIN t_user u ON aj.c_uid = u.c_uid
                    INNER JOIN t_job_post jp ON jp.c_job_id = aj.c_job_id
                    INNER JOIN t_companies c ON jp.c_company_id = c.c_company_id
                    WHERE u.c_uid = @c_uid;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_uid", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_apply_job
                    {
                        c_application_id = Convert.ToInt32(dr["c_application_id"]),
                        c_job_id = Convert.ToInt32(dr["c_job_id"]),
                        job_Post = new Job_Post
                        {
                            c_job_title = dr["JobTitle"]?.ToString(),
                            c_job_location = dr["Location"]?.ToString(),
                        },
                        company = new t_Company
                        {
                            c_company_name = dr["CompanyName"]?.ToString(),
                            c_company_logo = dr["CompanyLogo"]?.ToString()
                        }
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user applied job application:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<List<Job_Post>> GetJobTitles(int id)
    {
        List<Job_Post> tt = new List<Job_Post>();
        try
        {
            await _conn.OpenAsync();
            string qry = "select c_job_title from t_job_post where c_company_id=@c_company_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new Job_Post
                    {
                        c_job_title = dr["c_job_title"].ToString(),
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while fetching job title:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> InterviewSchedule(t_interview_schedule interview_Schedule)
    {
        try
        {
            Random random = new Random();
            int num = random.Next(1000, 10000); // generates a number from 1000 to 9999
            string url = "http://localhost:5000/UserDashboard/Meeting?roomID=" + num;
            interview_Schedule.c_meeting_url = url;
            await _conn.OpenAsync();
            string qry = "insert into t_interview_schedule (c_user_id,c_interview_date,c_interview_time,c_company_id,c_meeting_url) values (@c_user_id,@c_interview_date,@c_interview_time,@c_company_id,@c_meeting_url)";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_user_id", interview_Schedule.c_user_id);
            cmd.Parameters.AddWithValue("@c_interview_date", DateTime.Parse(interview_Schedule.c_interview_date));
            cmd.Parameters.AddWithValue("@c_interview_time", TimeSpan.Parse(interview_Schedule.c_interview_time));
            cmd.Parameters.AddWithValue("@c_company_id", interview_Schedule.c_company_id);
            cmd.Parameters.AddWithValue("@c_meeting_url", url);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while schedule interview:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<List<t_interview_schedule>> GetInterviews_ScheduleByCompany(int id)
    {
        List<t_interview_schedule> tt = new List<t_interview_schedule>();
        try
        {
            await _conn.OpenAsync();
            string qry = @"select ts.c_meeting_url,ts.c_interview_id,ts.c_interview_date,ts.c_interview_time,u.c_uid,u.c_fullname,u.c_email
                        from t_interview_schedule ts
                        inner join t_user u
                        on ts.c_user_id = u.c_uid
                        where ts.c_company_id = @c_company_id;";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_company_id", id);
            var dr = await cmd.ExecuteReaderAsync();
            if (dr.HasRows)
            {
                while (await dr.ReadAsync())
                {
                    tt.Add(new t_interview_schedule
                    {
                        user = new t_user
                        {
                            c_userId = Convert.ToInt32(dr["c_uid"]),
                            c_fullName = dr["c_fullName"].ToString(),
                            c_email = dr["c_email"].ToString(),
                        },
                        c_interview_id = Convert.ToInt32(dr["c_interview_id"]),
                        c_interview_date = dr["c_interview_date"].ToString(),
                        c_interview_time = dr["c_interview_time"].ToString(),
                        c_meeting_url = dr["c_meeting_url"].ToString(),
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while fetching schedule interviews by company:" + ex.Message);
        }
        finally
        {
            await _conn.CloseAsync();
        }
        return tt;
    }

    public async Task<int> UpdateInterviewSchedule(t_interview_schedule interview_Schedule)
    {
        try
        {
            Random random = new Random();
            int num = random.Next(1000, 10000); // generates a number from 1000 to 9999
            string url = "http://localhost:5000/UserDashboard/Meeting?roomID=" + num;
            interview_Schedule.c_meeting_url = url;
            await _conn.OpenAsync();
            string qry = "update t_interview_schedule set c_interview_date=@c_interview_date,c_interview_time=@c_interview_time where c_interview_id=@c_interview_id";
            var cmd = new NpgsqlCommand(qry, _conn);
            cmd.Parameters.AddWithValue("@c_interview_date", DateTime.Parse(interview_Schedule.c_interview_date));
            cmd.Parameters.AddWithValue("@c_interview_time", TimeSpan.Parse(interview_Schedule.c_interview_time));
            cmd.Parameters.AddWithValue("@c_interview_id", interview_Schedule.c_interview_id);
            await cmd.ExecuteNonQueryAsync();
            return 1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while updating interview schedule:" + ex.Message);
            return 0;
        }
        finally
        {
            await _conn.CloseAsync();
        }
    }

    public async Task<int> UserAppliedStatus(int userid, int jobid)
    {
        try
        {
            await _conn.OpenAsync();
            string qry = "select * from t_apply_jobs where c_job_id=@c_job_id and c_uid = @c_uid";
            var cmd = new NpgsqlCommand(qry,_conn);
            cmd.Parameters.AddWithValue("@c_uid",userid);
            cmd.Parameters.AddWithValue("@c_job_id",jobid);
            var dr = await cmd.ExecuteReaderAsync();
            if(dr.HasRows){
                return 1;
            }
            return -1;
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("Error while getting user applied job status:"+ex.Message);
            return 0;
        }
        finally{
            await _conn.CloseAsync();
        }
    }
}