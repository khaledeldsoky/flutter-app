using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Authentication;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using App.Models;

namespace App.Services
{
  public class SingUpServices
  {
    
    List<SingUpModel> users = new List<SingUpModel>();
    private readonly string _connectionString;
    public SingUpServices(IConfiguration configuration)
    {
      _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    

    public void AddUser(SingUpModel user)
    {
      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      // 1️⃣ Check if email already exists
      string checkEmail = "select count(*) from signup where email = @e";

      using MySqlCommand commandEmail = new MySqlCommand(checkEmail, connection);
      commandEmail.Parameters.AddWithValue("@e", user.email);

      int exists = Convert.ToInt32(commandEmail.ExecuteScalar());
      if (exists > 0)
        throw new Exception("Email already exists"); ;


      // 2️⃣ Hash password
      string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.passowrd);


      // 3️⃣ Insert user
      string inserSql = "insert into  signup (username , email , password  ,phone , token)" + "Values(@n,@e,@pass,@ph,@t)";
      using MySqlCommand insercommand = new MySqlCommand(inserSql, connection);
      insercommand.Parameters.AddWithValue("@n", user.name);
      insercommand.Parameters.AddWithValue("@e", user.email);
      insercommand.Parameters.AddWithValue("@pass", hashedPassword);
      insercommand.Parameters.AddWithValue("@ph", user.phone);
      insercommand.Parameters.AddWithValue("@t", user.token);

      insercommand.ExecuteNonQuery();
    }

    public List<SingUpModel> GetAllUsers()
    {
      SingUpModel user = new SingUpModel();
      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      string sql = "select * from signup";
      using MySqlCommand command = new MySqlCommand(sql, connection);

      using MySqlDataReader dataReader = command.ExecuteReader();

      while (dataReader.Read())
      {
        user.name = dataReader.GetString("username");
        user.email = dataReader.GetString("email");
        user.phone = dataReader.GetString("phone");
        users.Add(user);
      }

      return users;
    }

    public bool LogIn(string Email , string Password)
    {
      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      string sql = "SELECT password FROM signup WHERE email = @e";   

      using MySqlCommand command = new MySqlCommand(sql,connection);

      command.Parameters.AddWithValue("@e",Email);

      var HashedPassword = command.ExecuteScalar().ToString();

      bool Success = BCrypt.Net.BCrypt.Verify(Password,HashedPassword);

      return Success;
    }
  }
}