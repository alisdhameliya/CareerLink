using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Repositories.Interface;
using Repositories.Model;

namespace Repositories.Implementation
{
    public class PostedJobRepository : IPostedJobInterface
    {
        private readonly string _connectionString;

        public PostedJobRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("pgcon");
        }
        public async Task<List<Job_Post>> GetJobDetails()
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
              INNER JOIN t_companies c ON jb.c_company_id = c.c_company_id", conn);
        
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

        public async Task<List<t_ViewJobs>> GetAllJobs()
        {
            List<t_ViewJobs> tw = new List<t_ViewJobs>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(@"select jb.c_job_id,jb.c_job_title,c.c_company_name,c.c_company_logo,jb.c_job_experience,jb.c_salary_range,jb.c_job_location,jb.c_skills,jb.c_post_date,jb.c_company_id,c.c_company_email,c.c_company_phone,c.c_industry,c.c_website,c.c_company_address
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
                            c_company_logo = dr["c_company_logo"].ToString(),
                            c_company_email = dr["c_company_email"].ToString(),
                            c_company_phone_number = dr["c_company_phone"].ToString(),
                            c_company_address = dr["c_company_address"].ToString(),
                            c_company_industry = dr["c_industry"].ToString(),
                            c_company_website = dr["c_website"].ToString()
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

        public async Task<Job_Post> GetJobDescription(int id)
        {
            var jobPosts = new List<Job_Post>();
            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                using var cmd = new NpgsqlCommand(
                    @"SELECT jb.c_job_id, jb.c_job_title, jb.c_job_desc, jb.c_post_date, jb.c_job_location, 
                     jb.c_job_type, jb.c_job_experience, jb.c_salary_range, jb.c_vacancy, 
                     jb.c_qualification_title, jb.c_skills, 
                     d.c_dept_id, d.c_dept_name, 
                     c.c_company_name, c.c_company_logo,c.c_company_email,c.c_company_phone, c.c_industry, c.c_website, c.c_company_address
              FROM t_job_post jb
              INNER JOIN t_department d ON jb.c_dept_id = d.c_dept_id
              INNER JOIN t_companies c ON jb.c_company_id = c.c_company_id
              WHERE jb.c_job_id=@Id;", conn);
                cmd.Parameters.AddWithValue("@Id", id);

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
                        department = new t_department
                        {
                            c_dept_id = Convert.ToInt32(reader["c_dept_id"]),
                            c_deptname = reader["c_dept_name"].ToString()
                        },
                        company = new t_Company
                        {
                            c_company_name = reader["c_company_name"].ToString(),
                            c_company_logo = reader["c_company_logo"].ToString(),
                            c_company_email = reader["c_company_email"].ToString(),
                            c_company_phone_number = reader["c_company_phone"].ToString(),
                            c_company_address = reader["c_company_address"].ToString(),
                            c_company_industry = reader["c_industry"].ToString(),
                            c_company_website = reader["c_website"].ToString()
                        }
                    };

                    return post;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetJobDescription method in JobPostRepository: {ex.Message}\nStack Trace: {ex.StackTrace}");
                return null;
            }
        }
    }
}