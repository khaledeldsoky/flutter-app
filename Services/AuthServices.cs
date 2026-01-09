using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using App.Models;

namespace App.Services
{
  public class AuthServices
  {


    private readonly string _connectionString;
    public AuthServices(IConfiguration configuration)
    {
      _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public bool Register(RegisterRequest register)
    {
      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      // 1️⃣ Check if email already exists
      string checkEmail = "select count(*) from users where email = @e";

      using MySqlCommand commandEmail = new MySqlCommand(checkEmail, connection);
      commandEmail.Parameters.AddWithValue("@e", register.Email);

      int exists = Convert.ToInt32(commandEmail.ExecuteScalar());
      if (exists > 0)
        return false ;


      // 2️⃣ Hash password
      

      // 3️⃣ Insert user
      string inserSql = "insert into  users (username , email , password  ,phone )" + "Values(@n,@e,@pass,@ph)";
      string hashedPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);

      using MySqlCommand insercommand = new MySqlCommand(inserSql, connection);
      insercommand.Parameters.AddWithValue("@n", register.Username);
      insercommand.Parameters.AddWithValue("@e", register.Email);
      insercommand.Parameters.AddWithValue("@pass", hashedPassword);
      insercommand.Parameters.AddWithValue("@ph", register.Phone);

      insercommand.ExecuteNonQuery();

      return true;
    }

    public List<UserDto> GetAllUsers()
    {
      List<UserDto> UsersDto = new List<UserDto>();

      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      string sql = "SELECT email , username , phone FROM users";

      using MySqlCommand command = new MySqlCommand(sql, connection);

      using MySqlDataReader dataReader = command.ExecuteReader();

      while (dataReader.Read())
      {
        UserDto UserDto = new UserDto();

        UserDto.Username = dataReader.GetString("username");
        UserDto.Email = dataReader.GetString("email");
        UserDto.Phone = dataReader.GetString("phone");
        UsersDto.Add(UserDto);
      }
      
      return UsersDto;
    }

    public bool LogIn(LoginRequest login)
    {
      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      string sql = "SELECT password FROM users WHERE email = @e";

      using MySqlCommand command = new MySqlCommand(sql, connection);

      command.Parameters.AddWithValue("@e", login.Email);

      var result = command.ExecuteScalar();

      if (result ==  null)
        return false;

      var HashedPassword = result.ToString();

      bool Success = BCrypt.Net.BCrypt.Verify(login.Password, HashedPassword);

      return Success;
    }
  }
}