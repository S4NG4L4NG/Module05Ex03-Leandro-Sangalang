using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Module07dataaccess.Model;
using MySql.Data.MySqlClient;

namespace Module07dataaccess.Services
{
    public class PersonalService
    {
        public readonly string _connectionString;

        public PersonalService()
        {
            var dbService = new DataBaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Personal>> GetAllPersonalsAsync()
        {
            var personalService = new List<Personal>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var cmd = new MySqlCommand("SELECT * FROM tblemployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalService.Add(new Personal
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Name = reader.GetString("Name"),
                            Address = reader.GetString("Address"),
                            email = reader.GetString("email"),
                            ContactNo = reader.GetString("ContactNo"),
                        });
                    }
                }
            }
            return personalService;
        }

        public async Task<bool> AddPersonalAsync(Personal newPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new MySqlCommand("INSERT INTO tblemployee (Name, Address, email, ContactNo) VALUES (@Name, @Address, @email, @ContactNo)", conn);
                    cmd.Parameters.AddWithValue("@Name", newPerson.Name);
                    cmd.Parameters.AddWithValue("@Address", newPerson.Address);
                    cmd.Parameters.AddWithValue("@email", newPerson.email);
                    cmd.Parameters.AddWithValue("@ContactNo", newPerson.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding employee record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePersonalAsync(int EmployeeID)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblemployee WHERE EmployeeID = @EmployeeID", conn);
                    cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting employee record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdatePersonalAsync(Personal updatedPerson)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new MySqlCommand(
                        "UPDATE tblemployee SET Name = @Name, Address = @Address, email = @Email, ContactNo = @ContactNo WHERE EmployeeID = @EmployeeID",
                        conn);

                    cmd.Parameters.AddWithValue("@Name", updatedPerson.Name);
                    cmd.Parameters.AddWithValue("@Address", updatedPerson.Address);
                    cmd.Parameters.AddWithValue("@Email", updatedPerson.email);
                    cmd.Parameters.AddWithValue("@ContactNo", updatedPerson.ContactNo);
                    cmd.Parameters.AddWithValue("@EmployeeID", updatedPerson.EmployeeID);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating employee record: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Personal>> SearchPersonalsAsync(string searchTerm)
        {
            var personalService = new List<Personal>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var cmd = new MySqlCommand(
                        "SELECT * FROM tblemployee WHERE Name LIKE @SearchTerm OR Address LIKE @SearchTerm OR email LIKE @SearchTerm OR ContactNo LIKE @SearchTerm",
                        conn);
                    cmd.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            personalService.Add(new Personal
                            {
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                Name = reader.GetString("Name"),
                                Address = reader.GetString("Address"),
                                email = reader.GetString("email"),
                                ContactNo = reader.GetString("ContactNo"),
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching employee records: {ex.Message}");
            }
            return personalService;
        }
    }


}


