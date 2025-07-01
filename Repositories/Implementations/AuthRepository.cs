using System;
using System.Threading.Tasks;
using BCrypt.Net;
using Npgsql;
// using Org.BouncyCastle.Crypto.Generators;
using Repositories.Interfaces;
using Repositories.Model;

namespace Repositories.Implementation
{
    public class AuthRepository : IAuthInterface
    {
        private readonly NpgsqlConnection _connection;

        public AuthRepository(NpgsqlConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }
        public async Task<t_user?> Login(vm_Login login) // ✅ Return nullable user
        {
            Console.WriteLine("🔍 Searching user in database...");
            t_user? user = null;  // ✅ Initialize as null

            const string query = "SELECT * FROM t_user WHERE c_email = @c_email AND c_role = @c_role;";

            try
            {
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_email", login.c_email);
                    cmd.Parameters.AddWithValue("@c_role", login.c_userRole);

                    await _connection.OpenAsync();
                    Console.WriteLine("✅ Database connection opened.");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Console.WriteLine("✅ User record found.");
                            string storedHashedPassword = reader["c_password"].ToString();
                            Console.WriteLine("🔑 Hashed Password from DB: " + storedHashedPassword); // ✅ Debugging added

                            // Verify input password against stored hash
                            if (BCrypt.Net.BCrypt.Verify(login.c_password, storedHashedPassword))
                            {
                                user = new t_user  // ✅ Assign only when user is found
                                {
                                    c_userId = reader.GetInt32(reader.GetOrdinal("c_uid")),
                                    c_username = reader.GetString(reader.GetOrdinal("c_username")),
                                    c_fullName = reader.GetString(reader.GetOrdinal("c_fullname")),
                                    c_email = reader.GetString(reader.GetOrdinal("c_email")),
                                    c_phoneNumber = reader.GetString(reader.GetOrdinal("c_phone_number")),
                                    c_gender = reader.GetString(reader.GetOrdinal("c_gender")),
                                    c_profileImage = reader.IsDBNull(reader.GetOrdinal("c_image")) ? "default.png" : reader.GetString(reader.GetOrdinal("c_image")),
                                    c_userRole = reader.GetString(reader.GetOrdinal("c_role")),
                                    c_password = storedHashedPassword  // ✅ Ensure password is assigned
                                };
                                Console.WriteLine("✅ User object created successfully.");
                            }
                            else
                            {
                                Console.WriteLine("❌ Password verification failed.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("❌ No user found with this email and role.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error in Login: {e.Message}");
            }
            finally
            {
                await _connection.CloseAsync();
            }

            return user;  // ✅ Returns null if user not found
        }
        public async Task<int> GetUserEmail(string email)
        {
            try
            {
                await _connection.OpenAsync();
                string query = "SELECT COUNT(*) FROM t_user WHERE c_email=@c_email";
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_email", email);
                    var count = (long)await cmd.ExecuteScalarAsync();
                    return count > 0 ? 1 : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in getting Registered email: " + ex.Message);
                return -1;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
        public async Task<bool> ChangePassword(string userEmail, string newPassword)
        {
            try
            {
                await _connection.OpenAsync();

                // Hash the new password before storing it
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                Console.WriteLine($"Hashed Password: {hashedPassword}"); // Debugging

                string query = "UPDATE t_user SET c_password = @c_password WHERE c_email = @c_email";
                using (var cmd = new NpgsqlCommand(query, _connection))
                {
                    cmd.Parameters.AddWithValue("@c_password", hashedPassword); // Store hashed password
                    cmd.Parameters.AddWithValue("@c_email", userEmail);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating password: {ex.Message}");
                return false;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }



        public async Task<int> Register(t_user1 user)
        {
            try
            {
                await _connection.OpenAsync();
                string query1 = "SELECT COUNT(*) FROM t_user WHERE c_email=@c_email";
                using (NpgsqlCommand cmd1 = new NpgsqlCommand(query1, _connection))
                {
                    cmd1.Parameters.AddWithValue("@c_email", user.c_email);
                    var count = (long)await cmd1.ExecuteScalarAsync();
                    if (count > 0)
                    {
                        return 0;
                    }
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.c_password);

                const string query = @"
                    INSERT INTO t_user (c_username, c_fullname, c_email, c_password, c_phone_number, c_gender, c_image, c_role) 
                    VALUES (@Username, @FullName, @Email, @Password, @PhoneNumber, @Gender, @Image, @Role)";

                await using var cmd = new NpgsqlCommand(query, _connection);
                cmd.Parameters.AddWithValue("@Username", user.c_username);
                cmd.Parameters.AddWithValue("@FullName", user.c_fullName);
                cmd.Parameters.AddWithValue("@Email", user.c_email);
                cmd.Parameters.AddWithValue("@Password", hashedPassword);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.c_phoneNumber);
                cmd.Parameters.AddWithValue("@Gender", user.c_gender);
                cmd.Parameters.AddWithValue("@Image", (object?)user.c_profileImage ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", user.c_userRole);
                int result = await cmd.ExecuteNonQueryAsync();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Register: {ex.Message}");
                return -1;
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}
