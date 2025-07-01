using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Model;
using Repositories.Models;

namespace Repositories.Implimentations
{
    public class CompanyRepository : ICompanyInterface
    {
        private readonly NpgsqlConnection _conn;

        public CompanyRepository(NpgsqlConnection connection)
        {
            _conn = connection;
        }

         public async Task<Job_Post> GetCompanyName(int companyId)
        {
            Job_Post job = new Job_Post();
            job.company = new t_Company(); // Ensure the company object is initialized

            try
            {
                await _conn.OpenAsync();
                string qry = "SELECT c_company_name FROM t_companies WHERE c_company_id = @c_company_id";

                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_company_id", companyId);
                    using (NpgsqlDataReader rd = await cmd.ExecuteReaderAsync())
                    {
                        if (await rd.ReadAsync()) // Use async version for better performance
                        {
                            job.company.c_company_name = rd["c_company_name"]?.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetCompanyName method: " + ex.Message);
            }
            finally
            {
                await _conn.CloseAsync();
            }

            return job;
        }

        public async Task<int> GetCompanyId(int Userid)
        {
            try
            {
                await _conn.OpenAsync();
                string qry = "select c_company_id from t_companies where c_owner_id = @c_owner_id";
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_owner_id", Userid);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (System.Exception)
            {
                Console.WriteLine("Error in GetCompanyId method");
                return 0;                
            }
            finally{
                await _conn.CloseAsync();
            }
        }

        public async Task<t_user> GetRecruiterById(int id)
        {
            var qry = @"SELECT * FROM t_user WHERE c_uid = @id";

            try
            {
                await _conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = await cmd.ExecuteReaderAsync();
                    if (reader.Read())
                    {
                        return new t_user
                        {
                            c_userId = reader.GetInt32(0),
                            c_username = reader.GetString(1),
                            c_fullName = reader.GetString(2),
                            c_email = reader.GetString(3),
                            c_password = reader.GetString(4),
                            c_phoneNumber = reader.GetString(5),
                            c_gender = reader.GetString(6),
                            c_profileImage = reader.IsDBNull(7) ? null : reader.GetString(7),
                            c_userRole = reader.GetString(8)
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}");
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return null;
        }

        public async Task<int> RegisterCompany(t_companies company)
        {
            try
            {
                await _conn.OpenAsync();

                var query1 = "SELECT 1 FROM t_companies WHERE c_owner_id = @c_owner_id LIMIT 1";
                using (var cmd = new NpgsqlCommand(query1, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_owner_id", company.c_owner_id);
                    var exists = await cmd.ExecuteScalarAsync();
                    if (exists != null)
                    {
                        return 0;
                    }
                }

                object? legalDocumentsArray = company.c_legal_documents?.Length > 0 ? company.c_legal_documents : DBNull.Value;
                string logoPath = company.c_company_logo ?? DBNull.Value.ToString();

                var query2 = @"INSERT INTO t_companies 
                    (c_company_name, c_owner_id, c_company_email, c_company_phone, c_company_address, 
                     c_company_reg_number, c_tax_id_number, c_industry, c_website,  
                     c_legal_documents, c_company_logo)
                    VALUES 
                    (@CompanyName, @OwnerId, @CompanyEmail, @CompanyPhone, @CompanyAddress, 
                     @CompanyRegNumber, @TaxIdNumber, @Industry, @Website, 
                     @LegalDocuments, @LogoPath)";

                using (var cmd = new NpgsqlCommand(query2, _conn))
                {
                    cmd.Parameters.AddWithValue("@CompanyName", company.c_company_name);
                    cmd.Parameters.AddWithValue("@OwnerId", company.c_owner_id);
                    cmd.Parameters.AddWithValue("@CompanyEmail", company.c_company_email);
                    cmd.Parameters.AddWithValue("@CompanyPhone", company.c_company_phone);
                    cmd.Parameters.AddWithValue("@CompanyAddress", company.c_company_address);
                    cmd.Parameters.AddWithValue("@CompanyRegNumber", company.c_company_reg_number ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaxIdNumber", company.c_tax_id_number ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Industry", company.c_industry ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Website", company.c_website ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@LegalDocuments", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text, legalDocumentsArray);
                    cmd.Parameters.AddWithValue("@LogoPath", logoPath);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? 1 : -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering company: {ex.Message}");
                return -1;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }

        public async Task<List<t_companies>> GetMandatoryFields()
        {
            List<t_companies> tt = new List<t_companies>();
            try
            {
                await _conn.OpenAsync();
                string qry = "select c_company_name, c_company_email, c_company_reg_number, c_tax_id_number from t_companies";
                var cmd = new NpgsqlCommand(qry, _conn);
                var dr = await cmd.ExecuteReaderAsync();
                if(dr.HasRows){
                    while (await dr.ReadAsync())
                    {
                        tt.Add(new t_companies{
                            c_company_name = dr["c_company_name"]?.ToString(),
                            c_company_email = dr["c_company_email"]?.ToString(),
                            c_company_reg_number = dr["c_company_reg_number"]?.ToString(),
                            c_tax_id_number = dr["c_tax_id_number"]?.ToString()
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error in GetMandatoryFields method: {ex.Message}");
            }
            finally{
                await _conn.CloseAsync();
            }
            return tt;
        }

        public async Task<int> GetCompanyStatus(int id)
        {
            try
            {
                await _conn.OpenAsync();
                string qry = "SELECT c_verified_status FROM t_companies WHERE c_company_id = @c_company_id";
                using (var cmd = new NpgsqlCommand(qry, _conn))
                {
                    cmd.Parameters.AddWithValue("@c_company_id", id);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (System.Exception)
            {
                
                Console.WriteLine("Error in GetCompanyStatus method");
                return 0;
            }
            finally
            {
                await _conn.CloseAsync();
            }
        }
    
        
    }
}