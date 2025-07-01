using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Model;
using Microsoft.Extensions.Configuration;

namespace Repositories.Implementations
{
    public class JobPostRepository : IJobPostInterface
    {
        private readonly string _connectionString;

        public JobPostRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("pgcon");
        }

        public async Task<int> CreateJob(Job_Post job)
        {
            try
            {
                if (job == null)
                {
                    Console.WriteLine("Job_Post object is null.");
                    return 0; // Failure
                }

                if (job.c_company_id == 0)
                {
                    Console.WriteLine("Company ID is null or empty.");
                    return 0; // Failure
                }

                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                // First, check if the recruiter's company is approved
                using var checkCmd = new NpgsqlCommand(@"
            SELECT COUNT(1) FROM t_companies 
            WHERE c_company_id = @CompanyId AND c_verified_status = 'true';", conn);

                checkCmd.Parameters.AddWithValue("@CompanyId", job.c_company_id);

                var result = await checkCmd.ExecuteScalarAsync();
                var isApproved = result != null ? (long)result : 0;

                if (isApproved == 0)
                {
                    Console.WriteLine("Recruiter's company is not approved. Job post cannot be created.");
                    return -1; // Return -1 to indicate the company is not approved
                }

                // If approved, insert the job post
                using var cmd = new NpgsqlCommand(@"
            INSERT INTO t_job_post ( 
                c_job_title, 
                c_job_desc,
                c_job_location, 
                c_job_type, 
                c_job_experience, 
                c_salary_range, 
                c_vacancy, 
                c_dept_id, 
                c_qualification_title, 
                c_skills, 
                c_company_id
            ) 
            VALUES (
                @Title, @Description, @Location, @Type, 
                @Experience, @SalaryRange, @Vacancy, 
                @DId, @Qualification, @Skills, @CompanyId
            );", conn);

                cmd.Parameters.AddWithValue("@Title", job.c_job_title ?? string.Empty);
                cmd.Parameters.AddWithValue("@Description", job.c_job_desc ?? string.Empty);
                cmd.Parameters.AddWithValue("@Location", job.c_job_location ?? string.Empty);
                cmd.Parameters.AddWithValue("@Type", job.c_job_type ?? string.Empty);
                cmd.Parameters.AddWithValue("@Experience", job.c_job_experience);
                cmd.Parameters.AddWithValue("@SalaryRange", job.c_salary_range ?? string.Empty);
                cmd.Parameters.AddWithValue("@Vacancy", job.c_vacancy);
                cmd.Parameters.AddWithValue("@DId", job.c_dept_id);
                cmd.Parameters.AddWithValue("@Qualification", job.c_qualification_title ?? string.Empty);
                cmd.Parameters.AddWithValue("@Skills", job.c_skills ?? string.Empty);
                cmd.Parameters.AddWithValue("@CompanyId", job.c_company_id);

                await cmd.ExecuteNonQueryAsync();
                return 1; // Success
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateJob method in JobPostRepository: {ex.Message}");
                return 0; // Failure
            }
        }


        // public async Task<int> CreateJob(Job_Post job)
        // {
        //     try
        //     {
        //         using var conn = new NpgsqlConnection(_connectionString);
        //         await conn.OpenAsync();

        //         // **Check if job already exists for the same company**
        //         using var checkCmd = new NpgsqlCommand(@"
        //     SELECT COUNT(*) FROM t_job_post 
        //     WHERE c_job_title = @Title AND c_company_id = @CompanyId;", conn);

        //         checkCmd.Parameters.AddWithValue("@Title", job.c_job_title);
        //         checkCmd.Parameters.AddWithValue("@CompanyId", job.c_company_id);

        //         var existingJobCount = (long)await checkCmd.ExecuteScalarAsync(); // PostgreSQL COUNT() returns long

        //         if (existingJobCount > 0)
        //         {
        //             return -1; // Indicates job already exists
        //         }

        //         // **Insert the new job if not already present**
        //         using var cmd = new NpgsqlCommand(@"
        //     INSERT INTO t_job_post ( 
        //         c_job_title, 
        //         c_job_desc,
        //         c_job_location, 
        //         c_job_type, 
        //         c_job_experience, 
        //         c_salary_range, 
        //         c_vacancy, 
        //         c_dept_id, 
        //         c_qualification_title, 
        //         c_skills, 
        //         c_company_id
        //     ) 
        //     VALUES (
        //         @Title, @Description, @Location, @Type, 
        //         @Experience, @SalaryRange, @Vacancy, 
        //         @DId, @Qualification, @Skills, @CompanyId
        //     );", conn);

        //         cmd.Parameters.AddWithValue("@Title", job.c_job_title);
        //         cmd.Parameters.AddWithValue("@Description", job.c_job_desc);
        //         cmd.Parameters.AddWithValue("@Location", job.c_job_location);
        //         cmd.Parameters.AddWithValue("@Type", job.c_job_type);
        //         cmd.Parameters.AddWithValue("@Experience", job.c_job_experience);
        //         cmd.Parameters.AddWithValue("@SalaryRange", job.c_salary_range);
        //         cmd.Parameters.AddWithValue("@Vacancy", job.c_vacancy);
        //         cmd.Parameters.AddWithValue("@DId", job.c_dept_id);
        //         cmd.Parameters.AddWithValue("@Qualification", job.c_qualification_title);
        //         cmd.Parameters.AddWithValue("@Skills", job.c_skills);
        //         cmd.Parameters.AddWithValue("@CompanyId", job.c_company_id);

        //         await cmd.ExecuteNonQueryAsync();
        //         return 1;
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error in CreateJob method in JobPostRepository: {ex.Message}");
        //         return 0;
        //     }
        // }


        public async Task<int> DeleteJob(int id)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand("DELETE FROM t_job_post WHERE c_job_id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0 ? 1 : 0; // Return 1 only if a row was deleted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteJob method in JobPostRepository: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> EditJob(int id, Job_Post job)
        {
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(@"
                    UPDATE t_job_post SET 
                        c_job_title = @Title, 
                        c_job_desc = @Description,
                        c_job_location = @Location, 
                        c_job_type = @Type, 
                        c_job_experience = @Experience, 
                        c_salary_range = @SalaryRange, 
                        c_vacancy = @Vacancy, 
                        c_dept_id = @DId, 
                        c_qualification_title = @Qualification, 
                        c_skills = @Skills, 
                        c_company_id = @CompanyId
                    WHERE 
                        c_job_id = @JId;", conn);

                cmd.Parameters.AddWithValue("@JId", id);
                cmd.Parameters.AddWithValue("@Title", job.c_job_title);
                cmd.Parameters.AddWithValue("@Description", job.c_job_desc);
                cmd.Parameters.AddWithValue("@Location", job.c_job_location);
                cmd.Parameters.AddWithValue("@Type", job.c_job_type);
                cmd.Parameters.AddWithValue("@Experience", job.c_job_experience);
                cmd.Parameters.AddWithValue("@SalaryRange", job.c_salary_range);
                cmd.Parameters.AddWithValue("@Vacancy", job.c_vacancy);
                cmd.Parameters.AddWithValue("@DId", job.c_dept_id);
                cmd.Parameters.AddWithValue("@Qualification", job.c_qualification_title);
                cmd.Parameters.AddWithValue("@Skills", job.c_skills);
                cmd.Parameters.AddWithValue("@CompanyId", job.c_company_id);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0 ? 1 : 0; // Return 1 only if a row was updated
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EditJob method in JobPostRepository: {ex.Message}");
                return 0;
            }
        }

        // public async Task<List<Job_Post>> GetJobDetails(int id)
        // {
        //     var jobPosts = new List<Job_Post>();
        //     try
        //     {
        //         using var conn = new NpgsqlConnection(_connectionString);
        //         await conn.OpenAsync();
        //         using var cmd = new NpgsqlCommand(@"select jb.c_job_id,jb.c_job_title,jb.c_job_desc,jb.c_post_date,jb.c_job_location,jb.c_job_type,jb.c_job_experience,jb.c_salary_range,jb.c_vacancy,jb.c_qualification_title,jb.c_skills,d.c_dept_name,c.c_company_name,d.c_dept_id,c.c_company_id
        //             from t_job_post jb
        //             inner join t_department d
        //             on jb.c_dept_id = d.c_dept_id
        //             inner join t_companies c
        //             on jb.c_company_id=c.c_company_id
        //             where jb.c_company_id=@c_company_id;", conn);
        //         cmd.Parameters.AddWithValue("@c_company_id", id);
        //         using var reader = await cmd.ExecuteReaderAsync();

        //         while (await reader.ReadAsync())
        //         {
        //             var post = new Job_Post
        //             {
        //                 c_job_id = Convert.ToInt32(reader["c_job_id"]),
        //                 c_job_title = reader["c_job_title"].ToString(),
        //                 c_job_desc = reader["c_job_desc"].ToString(),
        //                 c_job_location = reader["c_job_location"].ToString(),
        //                 c_job_type = reader["c_job_type"].ToString(),
        //                 c_job_experience = (int)reader["c_job_experience"],
        //                 c_salary_range = reader["c_salary_range"].ToString(),
        //                 c_vacancy = (int)reader["c_vacancy"],
        //                 c_dept_id = (int)reader["c_dept_id"],
        //                 c_qualification_title = reader["c_qualification_title"].ToString(),
        //                 c_skills = reader["c_skills"].ToString(),
        //                 c_post_date = reader["c_post_date"].ToString(),
        //                 c_company_id = Convert.ToInt32(reader["c_company_id"]),
        //                 department = new t_department
        //                 {
        //                     c_dept_id = Convert.ToInt32(reader["c_dept_id"]),
        //                     c_deptname = reader["c_dept_name"].ToString(),
        //                 },
        //                 company = new t_Company{
        //                     c_company_name = reader["c_company_name"].ToString(),
        //                     c_company_logo = reader["c_company_logo"].ToString()
        //                 }
        //             };
        //             jobPosts.Add(post);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error in GetJobDetails method in JobPostRepository: {ex.Message}");
        //     }
        //     return jobPosts; // Return even if empty on error
        // }

        public async Task<List<Job_Post>> GetJobDetails(int id)
        {
            var jobPosts = new List<Job_Post>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(
                    @"SELECT jb.c_job_id, jb.c_job_title, jb.c_job_desc, jb.c_post_date, jb.c_job_location, 
                     jb.c_job_type, jb.c_job_experience, jb.c_salary_range, jb.c_vacancy, 
                     jb.c_qualification_title, jb.c_skills, d.c_dept_name, c.c_company_name, 
                     d.c_dept_id, c.c_company_id, c.c_company_logo
              FROM t_job_post jb
              INNER JOIN t_department d ON jb.c_dept_id = d.c_dept_id
              INNER JOIN t_companies c ON jb.c_company_id = c.c_company_id
              WHERE jb.c_company_id = @c_company_id;", conn);
                cmd.Parameters.AddWithValue("@c_company_id", id);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var post = new Job_Post
                    {
                        c_job_id = Convert.ToInt32(reader["c_job_id"]),
                        c_job_title = reader["c_job_title"].ToString(),
                        c_job_desc = reader["c_job_desc"].ToString(),
                        c_job_location = reader["c_job_location"].ToString(),
                        c_job_type = reader["c_job_type"].ToString(),
                        c_job_experience = (int)reader["c_job_experience"],
                        c_salary_range = reader["c_salary_range"].ToString(),
                        c_vacancy = (int)reader["c_vacancy"],
                        c_dept_id = (int)reader["c_dept_id"],
                        c_qualification_title = reader["c_qualification_title"].ToString(),
                        c_skills = reader["c_skills"].ToString(),
                        c_post_date = reader["c_post_date"].ToString(),
                        c_company_id = Convert.ToInt32(reader["c_company_id"]),
                        department = new t_department
                        {
                            c_dept_id = Convert.ToInt32(reader["c_dept_id"]),
                            c_deptname = reader["c_dept_name"].ToString()
                        },
                        company = new t_Company
                        {
                            c_company_name = reader["c_company_name"].ToString(),
                            c_company_logo = reader["c_company_logo"] != DBNull.Value ? reader["c_company_logo"].ToString() : null
                        }
                    };
                    jobPosts.Add(post);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetJobDetails method in JobPostRepository: {ex.Message}\nStack Trace: {ex.StackTrace}");
                throw; // Rethrow for debugging; remove in production if silent failure is intended
            }
            return jobPosts;
        }

        public async Task<List<t_department>> getAllDepartments()
        {
            var departments = new List<t_department>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT * FROM t_department", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var department = new t_department
                    {
                        c_dept_id = (int)reader["c_dept_id"],
                        c_deptname = reader["c_dept_name"].ToString()
                    };
                    departments.Add(department);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getAllDepartments method in JobPostRepository: {ex.Message}");
            }
            return departments;
        }

        public async Task<List<t_skills>> getAllSkills()
        {
            var skills = new List<t_skills>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand("SELECT * FROM t_skills", conn);
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var skill = new t_skills
                    {
                        c_skill_id = (int)reader["c_skill_id"],
                        c_skill_title = reader["c_skill_title"].ToString()
                    };
                    skills.Add(skill);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in getAllSkills method in JobPostRepository: {ex.Message}");
            }
            return skills;
        }

        public async Task<List<t_ViewJobs>> GetAllJobs()
        {
            List<t_ViewJobs> tw = new List<t_ViewJobs>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(@"select jb.c_job_id,jb.c_job_title,c.c_company_name,c.c_company_logo,jb.c_job_experience,jb.c_salary_range,jb.c_job_location,jb.c_skills,jb.c_post_date,jb.c_company_id
                from t_job_post jb
                inner join t_companies c
                on c.c_company_id= jb.c_company_id;", conn);
                using var dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    tw.Add(new t_ViewJobs
                    {
                        c_job_id = Convert.ToInt32(dr["c_job_id"]),
                        c_job_title = dr["c_job_title"].ToString(),
                        c_job_experience = Convert.ToInt32(dr["c_job_experience"]),
                        c_salary_range = dr["c_salary_range"].ToString(),
                        c_job_location = dr["c_job_location"].ToString(),
                        c_skills = dr["c_skills"].ToString(),
                        c_post_date = dr["c_post_date"].ToString(),
                        c_company_id = Convert.ToInt32(dr["c_company_id"]),
                        company = new t_Company
                        {

                            c_company_name = dr["c_company_name"].ToString(),
                            c_company_logo = dr["c_company_logo"].ToString()
                        }
                    });
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while fetching all jobs" + ex.Message);
            }
            return tw;
        }

        public async Task<Job_Post> GetOneJob(int id)
        {
            Job_Post jb = null;
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                // Modified query with JOIN
                using var cmd = new NpgsqlCommand(
                    "SELECT tjp.*, td.c_dept_name " +
                    "FROM t_job_post tjp " +
                    "LEFT JOIN t_department td ON tjp.c_dept_id = td.c_dept_id " +
                    "WHERE tjp.c_job_id = @c_job_id", conn);
                cmd.Parameters.AddWithValue("@c_job_id", id);
                using var dr = await cmd.ExecuteReaderAsync();
                if (await dr.ReadAsync()) // Use if instead of while since c_job_id is likely unique
                {
                    jb = new Job_Post
                    {
                        c_job_id = Convert.ToInt32(dr["c_job_id"]),
                        c_job_title = dr["c_job_title"].ToString(),
                        c_job_desc = dr["c_job_desc"].ToString(),
                        c_post_date = dr["c_post_date"].ToString(),
                        c_job_location = dr["c_job_location"].ToString(),
                        c_job_type = dr["c_job_type"].ToString(),
                        c_job_experience = Convert.ToInt32(dr["c_job_experience"]),
                        c_salary_range = dr["c_salary_range"].ToString(),
                        c_vacancy = Convert.ToInt32(dr["c_vacancy"]),
                        c_qualification_title = dr["c_qualification_title"].ToString(),
                        c_skills = dr["c_skills"].ToString(),
                        c_dept_id = Convert.ToInt32(dr["c_dept_id"]),
                        department = new t_department
                        {
                            c_dept_id = Convert.ToInt32(dr["c_dept_id"]),
                            c_deptname = dr["c_dept_name"] != DBNull.Value ? dr["c_dept_name"].ToString() : null
                        }
                    };
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error while getting one job: " + ex.Message);
            }
            return jb;
        }
    }
}