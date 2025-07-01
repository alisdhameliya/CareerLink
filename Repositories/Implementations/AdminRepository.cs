using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Model;
using Repositories.Models;

namespace Repositories.Implimentation
{
    public class AdminRepository : IAdminInterface
    {
        private readonly NpgsqlConnection _conn;

        public AdminRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }


        public async Task<List<t_user>> GetUsers()
        {
            List<t_user> users = new List<t_user>();
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT * FROM t_user";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new t_user
                            {
                                c_userId = reader.GetInt32(0),
                                c_username = reader.GetString(1),
                                c_fullName = reader.GetString(2),
                                c_email = reader.GetString(3),
                                c_phoneNumber = reader.GetString(5),
                                c_gender = reader.GetString(6),
                                c_profileImage = !reader.IsDBNull(7) ? reader.GetString(7) : "default-profile.png",
                                c_userRole = reader.GetString(8)
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
            return users;
        }

        // ✅ Delete a user safely
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                await _conn.OpenAsync();
                var query = "DELETE FROM t_user WHERE c_uid = @id";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<int> GetUsersCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_user where c_role='Candidate'";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<int> GetRecruitersCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_user WHERE c_role = 'Recruiter'";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<List<t_recruiter>> GetRecruiters()
        {
            List<t_recruiter> recruiters = new List<t_recruiter>();
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT * FROM t_companies";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            recruiters.Add(new t_recruiter
                            {
                                c_company_id = reader.GetInt32(0),
                                c_company_name = reader.GetString(1),
                                c_owner_id = reader.GetInt32(2),
                                c_company_email = reader.GetString(3),
                                c_company_phone = reader.GetString(4),
                                c_company_address = reader.GetString(5),
                                c_company_reg_number = reader.GetString(6),
                                c_tax_id_number = reader.GetString(7),
                                c_industry = reader.GetString(8),
                                c_website = reader.IsDBNull(9) ? null : reader.GetString(9),
                                c_verified_status = reader.IsDBNull(10) ? null : reader.GetBoolean(10),
                                c_legal_documents = reader.IsDBNull(11) ? null : reader.GetValue(11) as string[],
                                c_company_logo = reader.IsDBNull(12) ? null : reader.GetString(12)
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
            return recruiters;
        }
        // public async Task<bool> UpdateRecruiterStatus(int companyId, bool approved = true)
        // {
        //     try
        //     {
        //         await _conn.OpenAsync();
        //         var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = @companyId";

        //         using (var cmd = new NpgsqlCommand(query, _conn))
        //         {
        //             cmd.Parameters.AddWithValue("@verifiedStatus", approved);
        //             cmd.Parameters.AddWithValue("@companyId", companyId);

        //             int rowsAffected = await cmd.ExecuteNonQueryAsync();
        //             return rowsAffected > 0; // Returns true if update was successful
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error updating status: {ex.Message}");
        //         return false;
        //     }
        //     finally
        //     {
        //         await _conn.CloseAsync();
        //     }
        // }
        public async Task<bool> UpdateRecruiterStatus(int companyId, bool approved = true)
        {
            try
            {
                await _conn.OpenAsync();

                // Update the recruiter's status
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = @companyId";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyId", companyId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0) // If update was successful
                    {
                        // Fetch recruiter's email and company name
                        string recruiterEmail = "";
                        string companyName = "";

                        var getRecruiterQuery = "SELECT c_company_email, c_company_name FROM t_companies WHERE c_company_id = @companyId";
                        using (var cmd2 = new NpgsqlCommand(getRecruiterQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyId", companyId);
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    recruiterEmail = reader.GetString(0);
                                    companyName = reader.GetString(1);
                                }
                            }
                        }

                        // Send approval email
                        if (approved && !string.IsNullOrEmpty(recruiterEmail))
                        {
                            var emailService = new EmailService();
                            emailService.SendApprovalEmail(recruiterEmail, companyName);
                        }

                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating status: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<bool> DeleteRecruiter(int companyId)
        {
            try
            {
                await _conn.OpenAsync();
                var query = "DELETE FROM t_companies WHERE c_company_id = @companyId";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@companyId", companyId);
                    int affectedRows = await cmd.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting recruiter: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        // public async Task<bool> BulkUpdateRecruiterStatus(List<int> companyIds, bool approved = true)
        // {
        //     try
        //     {
        //         await _conn.OpenAsync();
        //         var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = ANY(@companyIds)";

        //         using (var cmd = new NpgsqlCommand(query, _conn))
        //         {
        //             cmd.Parameters.AddWithValue("@verifiedStatus", approved);
        //             cmd.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

        //             int rowsAffected = await cmd.ExecuteNonQueryAsync();
        //             return rowsAffected > 0; // Returns true if update was successful
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error updating status: {ex.Message}");
        //         return false;
        //     }
        //     finally
        //     {
        //         await _conn.CloseAsync();
        //     }
        // }
        public async Task<bool> BulkUpdateRecruiterStatus(List<int> companyIds, bool approved = true)
        {
            try
            {
                await _conn.OpenAsync();

                // Update recruiter status in bulk
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = ANY(@companyIds)";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        // Fetch all recruiter emails and company names
                        var getRecruitersQuery = "SELECT c_company_email, c_company_name FROM t_companies WHERE c_company_id = ANY(@companyIds)";
                        using (var cmd2 = new NpgsqlCommand(getRecruitersQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                            List<(string Email, string CompanyName)> recruiters = new List<(string, string)>();
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    recruiters.Add((reader.GetString(0), reader.GetString(1)));
                                }
                            }

                            // Send bulk approval emails
                            var emailService = new EmailService();
                            foreach (var recruiter in recruiters)
                            {
                                emailService.SendApprovalEmail(recruiter.Email, recruiter.CompanyName);
                            }
                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Bulk Approval: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        // public async Task<bool> BulkUpdateRecruiterStatusReject(List<int> companyIds, bool approved = false)
        // {
        //     try
        //     {
        //         await _conn.OpenAsync();
        //         var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = ANY(@companyIds)";

        //         using (var cmd = new NpgsqlCommand(query, _conn))
        //         {
        //             cmd.Parameters.AddWithValue("@verifiedStatus", approved);
        //             cmd.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

        //             int rowsAffected = await cmd.ExecuteNonQueryAsync();
        //             return rowsAffected > 0; // Returns true if update was successful
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error updating status: {ex.Message}");
        //         return false;
        //     }
        //     finally
        //     {
        //         await _conn.CloseAsync();
        //     }
        // }


        public async Task<bool> BulkUpdateRecruiterStatusReject(List<int> companyIds, string rejectionReason, bool approved = false)
        {
            try
            {
                await _conn.OpenAsync();

                // Update recruiter status in bulk
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = ANY(@companyIds)";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        // Fetch all recruiter emails, company names, and recruiter names
                        var getRecruitersQuery = "SELECT c_company_email, c_company_name, c_owner_id FROM t_companies WHERE c_company_id = ANY(@companyIds)";
                        using (var cmd2 = new NpgsqlCommand(getRecruitersQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyIds", companyIds.ToArray());

                            List<(string Email, string CompanyName, string RecruiterName)> recruiters = new List<(string, string, string)>();
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    string recruiterName = reader.IsDBNull(2) ? "Unknown" : reader.GetValue(2).ToString();
                                    recruiters.Add((reader.GetString(0), reader.GetString(1), recruiterName));
                                }
                            }

                            // Send bulk rejection emails
                            var emailService = new EmailService();
                            foreach (var recruiter in recruiters)
                            {
                                emailService.SendRejectionEmail(recruiter.Email, recruiter.CompanyName, recruiter.RecruiterName, rejectionReason);
                            }
                        }
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in Bulk Rejection: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
        public async Task<bool> UpdateRecruiterstatusReject(int companyId, string rejectionReason, bool approved = false)
        {
            try
            {
                await _conn.OpenAsync();

                // Update the recruiter's status
                var query = "UPDATE t_companies SET c_verified_status = @verifiedStatus WHERE c_company_id = @companyId";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@verifiedStatus", approved);
                    cmd.Parameters.AddWithValue("@companyId", companyId);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    if (rowsAffected > 0) // If update was successful
                    {
                        // Fetch recruiter's email, company name, and recruiter name
                        string recruiterEmail = "";
                        string companyName = "";
                        string recruiterName = "";

                        var getRecruiterQuery = "SELECT c_company_email, c_company_name, c_owner_id FROM t_companies WHERE c_company_id = @companyId";
                        using (var cmd2 = new NpgsqlCommand(getRecruiterQuery, _conn))
                        {
                            cmd2.Parameters.AddWithValue("@companyId", companyId);
                            using (var reader = await cmd2.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    recruiterEmail = reader.GetString(0);
                                    companyName = reader.GetString(1);
                                    recruiterName = reader.IsDBNull(2) ? "Unknown" : reader.GetValue(2).ToString(); // ✅ Fix here
                                }
                            }
                        }

                        // Send email notification
                        var emailService = new EmailService();
                        if (approved && !string.IsNullOrEmpty(recruiterEmail))
                        {
                            emailService.SendApprovalEmail(recruiterEmail, companyName);
                        }
                        else if (!approved && !string.IsNullOrEmpty(recruiterEmail))
                        {
                            emailService.SendRejectionEmail(recruiterEmail, companyName, recruiterName, rejectionReason);
                        }

                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating status: {ex.Message}");
                return false;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<List<t_recruiter>> PendingApproval()
        {
            List<t_recruiter> pendingRecruiters = new List<t_recruiter>();

            try
            {
                await _conn.OpenAsync();
                var query = "SELECT * FROM t_companies WHERE c_verified_status  IS NULL ";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync()) // Loop to fetch all pending recruiters
                        {
                            pendingRecruiters.Add(new t_recruiter
                            {
                                c_company_id = reader.GetInt32(0),
                                c_company_name = reader.GetString(1),
                                c_owner_id = reader.GetInt32(2),
                                c_company_email = reader.GetString(3),
                                c_company_phone = reader.GetString(4),
                                c_company_address = reader.GetString(5),
                                c_company_reg_number = reader.GetString(6),
                                c_tax_id_number = reader.GetString(7),
                                c_industry = reader.GetString(8),
                                c_website = reader.IsDBNull(9) ? null : reader.GetString(9),
                                c_verified_status = reader.IsDBNull(10) ? null : reader.GetBoolean(10),
                                c_legal_documents = reader.IsDBNull(11) ? null : reader.GetValue(11) as string[],
                                c_company_logo = reader.IsDBNull(12) ? null : reader.GetString(12)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching pending approvals: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return pendingRecruiters; // Returns an empty list if no records are found
        }

        public async Task<List<RegistrationStats>> GetRegistrationStats(DateTime startDate, DateTime endDate)
        {
            List<RegistrationStats> stats = new List<RegistrationStats>();
            try
            {
                await _conn.OpenAsync();

                var query = @"
                    WITH dates AS (
                        SELECT generate_series(@startDate, @endDate, '1 day'::interval)::date as date
                    ),
                    user_counts AS (
                        SELECT DATE(c_reg_date) as date, COUNT(*) as new_users
                        FROM t_user
                        WHERE c_reg_date BETWEEN @startDate AND @endDate
                        GROUP BY DATE(c_reg_date)
                    ),
                    company_counts AS (
                        SELECT DATE(c_reg_date) as date, COUNT(*) as new_companies
                        FROM t_companies
                        WHERE c_reg_date BETWEEN @startDate AND @endDate
                        GROUP BY DATE(c_reg_date)
                    )
                    SELECT 
                        d.date,
                        COALESCE(uc.new_users, 0) as new_users,
                        COALESCE(cc.new_companies, 0) as new_companies
                    FROM dates d
                    LEFT JOIN user_counts uc ON d.date = uc.date
                    LEFT JOIN company_counts cc ON d.date = cc.date
                    ORDER BY d.date";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            stats.Add(new RegistrationStats
                            {
                                Date = reader.GetDateTime(0),
                                NewUsers = reader.GetInt32(1),
                                NewCompanies = reader.GetInt32(2)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting registration stats: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return stats;
        }

        public async Task<UserDistribution> GetUserDistribution()
        {
            try
            {
                await _conn.OpenAsync();

                var query = @"
                    SELECT 
                        COUNT(*) FILTER (WHERE c_role = 'Candidate') as job_seekers,
                        COUNT(*) FILTER (WHERE c_role = 'Recruiter') as recruiters
                    FROM t_user";

                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UserDistribution
                            {
                                JobSeekers = reader.GetInt32(0),
                                Recruiters = reader.GetInt32(1)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user distribution: {ex.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return new UserDistribution();
        }

        public async Task<List<t_job>> GetJobs()
        {
            List<t_job> job = new List<t_job>();
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT c_job_id,c_job_title,c_job_location FROM t_job_post";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            job.Add(new t_job
                            {
                                c_job_id = reader.GetInt32(0),
                                c_job_title = reader.GetString(1),

                                c_job_location = reader.GetString(2)
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
            return job;
        }

        public async Task<int> GetJobPostCount()
        {
            try
            {
                await _conn.OpenAsync();
                var query = "SELECT COUNT(*) FROM t_job_post";
                using (var cmd = new NpgsqlCommand(query, _conn))
                {
                    return Convert.ToInt32(await cmd.ExecuteScalarAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

    }
}
