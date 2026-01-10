namespace App.Models
{
  public class ChangePassword
  {
    public string Email{set;get;}
    public string OldPassword{set;get;}
    public string NewPassword{set;get;}
  }
}