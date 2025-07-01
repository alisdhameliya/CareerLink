using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Repositories.Interfaces;
using Repositories.Models;

namespace Repositories.Implementations
{
    public class IndustryRepository : IIndustryInterface
    {
        private readonly NpgsqlConnection _conn;
        public IndustryRepository(NpgsqlConnection conn)
        {
            _conn = conn;
        }

        public async Task<List<Industry>> GetIndustriesAsync()
        {
            var industries = new List<Industry>();
            try
            {
                await _conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM industries", _conn))
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var industry = new Industry
                        {
                            id = reader.GetInt32(0),
                            industry_name = reader.GetString(1)
                        };
                        industries.Add(industry);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await _conn.CloseAsync();
            }
            return industries;
        }
    }
}