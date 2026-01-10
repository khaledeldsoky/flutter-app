namespace App.Models
{
  public class CreateOTP
  {
    public string id {set;get;}
    public string phone {set;get;}
  }

    public class VerifyOTP
  {
    public string OTP {set;get;}
  }
}

class Hepler
{
      public void CreateOTP(CreateOTP createOTP)
    {
      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      string sql = "INSERT INTO phone_otps (user_id, phone) VALUES (@id, @p);";
      using MySqlCommand command = new MySqlCommand(sql, connection);

      command.Parameters.AddWithValue("@id", createOTP.id);
      command.Parameters.AddWithValue("@p", createOTP.phone);

      command.ExecuteNonQuery();
    }

    public bool VerifyOTP(VerifyOTP verifyOTP)
    {
      using MySqlConnection connection = new MySqlConnection(_connectionString);
      connection.Open();

      string sqlCheckOTP = "SELECT expires_at from phone_otps WHERE otp_code =  @otp and  expires_at > NOW()";
      using MySqlCommand CheckOTP = new MySqlCommand(sqlCheckOTP, connection);
      CheckOTP.Parameters.AddWithValue("@otp",verifyOTP.OTP);
      int exists = Convert.ToInt32(CheckOTP.ExecuteScalar());

      if (exists > 0 )
        return true;

      return false;
    }
}
    

