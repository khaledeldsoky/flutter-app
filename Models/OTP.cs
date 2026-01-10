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