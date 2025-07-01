using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Interfaces;
using Npgsql;
using Repositories.Models;


namespace Repositories.Implementations
{
    public class CandidateRepository :  ICandidateInterface
    {
        private readonly NpgsqlConnection _conn;

        public CandidateRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }
        public async Task<List<Job_Post1>> GetJobs()
        {
            List<Job_Post1> jobs = new List<Job_Post1>();

            try
            {
                await _conn.OpenAsync();
                var query = "SELECT jp.*,cp.c_company_name,dp.c_dept_name,cp.c_company_logo FROM t_Job_Post jp join t_companies cp on jp.c_company_id = cp.c_company_id join t_department dp on jp.c_dept_id = dp.c_dept_id";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            jobs.Add(new Job_Post1
                            {
                                c_job_id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0),
                                c_job_title = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                                c_job_desc = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                c_post_date = reader["c_post_date"].ToString(),
                                c_job_location = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                c_job_type = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                                c_job_experience = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                c_salary_range = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                                c_vacancy = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                                c_dept_id = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                c_qualification_title = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                                c_skills = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                                c_company_id = reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                                c_company_name  = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                                c_dept_name = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                                c_company_logo = reader.IsDBNull(15) ? string.Empty : reader.GetString(15)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return jobs;
        }
    }
}