using BlazorApi.DTO;
using BlazorApi.Interfaces;
using BlazorApi.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System.Data;
using System.Formats.Asn1;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorApi.DataService
{
    public class DataService : IDataService
    {
        private readonly string _connectionstring;
        private readonly ILogger<DataService> _logger;
        private readonly IConfiguration _config;
        public DataService(string connectionstring, ILogger<DataService> logger)
        {
            _connectionstring = connectionstring ?? throw new ArgumentNullException(nameof(connectionstring));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private IDbConnection CreateConnection()
        {
            var connection = new MySqlConnection(_connectionstring);
            connection.Open();
            return connection;
        }

        public async Task<List<User>> GetAllUsers()
        {
            try
            {
                using var connection = CreateConnection();
                const string query = "SELECT * FROM Users";
                var users = await connection.QueryAsync<User>(query);
                return users.ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Users");
                throw;
            }

        }

        public async Task<User?> GetUserById(int userId)
        {
            try
            {
                using var connection = CreateConnection();
                const string query = "SELECT *  FROM Users WHERE UserId = @UserId";
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { UserId = userId });
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                throw new Exception("Database error occurred while retrieving the user.");
            }
        }

        public async Task<User> CreateUser(UserDto userDto)
        {
            try
            {
                using var connection = CreateConnection();

                // 1️⃣ Check if email exists
                const string checkEmailQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                var isExisting = await connection.ExecuteScalarAsync<int>(checkEmailQuery, new { userDto.Email });

                if (isExisting > 0)
                {
                    throw new Exception("User already exists.");
                }

                // 2️⃣ Hash password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

                // 3️⃣ Insert user and get the inserted ID
                const string query = @"
            INSERT INTO Users (Names, Email, Password, PhoneNumber) 
            VALUES (@Names, @Email, @Password, @PhoneNumber);
            SELECT LAST_INSERT_ID();";

                int userId = await connection.ExecuteScalarAsync<int>(query, new
                {
                    userDto.Names,
                    userDto.Email,
                    Password = hashedPassword,
                    userDto.PhoneNumber
                });

                if (userId == 0)
                {
                    throw new Exception("User registration failed. No ID returned.");
                }

                //  Fetch the inserted user
                const string getUserQuery = "SELECT UserId, Names, Email, PhoneNumber FROM Users WHERE UserId = @UserId";
                var createdUser = await connection.QuerySingleOrDefaultAsync<User>(getUserQuery, new { UserId = userId });

                if (createdUser == null)
                {
                    throw new Exception("User registration failed. User was not found after insertion.");
                }

                return createdUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User has not been registered successfully");
                throw new Exception("A database error occurred. Please try again later.");
            }
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> UpdateUserDetails(UpdateUserDto updateUserDto)
        {
            try
            {
                using var connection = CreateConnection();
                const string userExists = "SELECT * FROM Users WHERE UserId = @UserId";
                var user = await connection.QueryFirstOrDefaultAsync<User>(userExists, new  { updateUserDto.UserId });
                if (user == null)
                    {
                        return null;
                    }
                const string updatequery = @"UPDATE Users 
                        SET Names = @Names, Email=@Email, PhoneNumber= @PhoneNumber, Description=@Description
                        WHERE UserId = @UserId";
                int rowUpdated = await connection.ExecuteAsync(updatequery,new {
                    updateUserDto.Names,
                    updateUserDto.Email,
                    updateUserDto.PhoneNumber,
                    updateUserDto.Description,
                    updateUserDto.UserId

                });

                if (rowUpdated > 0)
                {
                    return await connection.QueryFirstOrDefaultAsync<User>(userExists, new { updateUserDto.UserId });
                }
                return null;
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "User Not Updated");
                throw;
            }
        }
        public async Task<bool> DeleteUser(int userId)
        {
            try
            {
                using var connection = CreateConnection();
                const string userFound = "SELECT * FROM Users WHERE UserId = @UserId";
                var user = await connection.QueryFirstOrDefaultAsync<User>(userFound, new { UserId = userId });
                if (user == null)
                {
                    return false;
                }
                const string deleteUserquery = "DELETE FROM Users WHERE UserId = @UserId";
                int isUserDeleted = await connection.ExecuteAsync(deleteUserquery, new { UserId = userId });
                user.IsDeleted = true;
                return true;

            }catch(Exception ex)
            {
                _logger.LogError(ex, $"Delete Operation Not SuccessFul for UserId: {userId}, UserId");
                throw new Exception("A database Error Ocurred Please Try Again");
            }
        }
    }
}
