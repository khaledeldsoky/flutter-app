using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using App.Services;
using App.Models;
using System.Net.Mail;

namespace App.Controllers
{
  [ApiController]
  [Route("api/auth/")]
  public class AuthControllers : ControllerBase
  {
    private readonly AuthServices _authServices;

    public AuthControllers(AuthServices authServices)
    {
      _authServices = authServices;
    }

    bool IsValidEmail(string Mail)
    {
      try
      {
        var Email = new MailAddress(Mail);
        return true;
      }
      catch (System.Exception)
      {
        return false;
      }
    }

    bool IsValidPhone(string Phone)
    {
      string[] EgyptionPrefix = { "010", "011", "012", "015" };
      if (Phone.Length == 11 && Phone.All(char.IsDigit))
      {
        foreach (var item in EgyptionPrefix)
        {
          if (Phone.StartsWith(item))
            return true;
        }
      }
      return false;
    }

    bool IsValidPassword(string Password)
    {
      if (Password.Length > 8 && Password.Any(char.IsUpper) && Password.Any(char.IsLower) && Password.Any(char.IsDigit) && Password.Any(ch => !char.IsLetterOrDigit(ch)))
      {
        return true;
      }
      return false;
    }


    [HttpPost("Register")]
    public IActionResult Register(RegisterRequest register)
    {

      // 1️⃣ Check email 
      if (string.IsNullOrWhiteSpace(register.Email))
        return BadRequest("Email is Requird");
      if (!IsValidEmail(register.Email))
        return BadRequest("Invalid email format");
      if (!_authServices.Register(register))
        return BadRequest("Email is Exist");

      // 2️⃣ Check password
      if (string.IsNullOrWhiteSpace(register.Password))
        return BadRequest("Password is Requird");
      if (!IsValidPassword(register.Password))
        return BadRequest("Invalid Password format");

      // 2️⃣ Check phone
      if (string.IsNullOrWhiteSpace(register.Phone))
        return BadRequest("Phone is required");
      if (!IsValidPhone(register.Phone))
        return BadRequest("Invalid phone format");



      try
      {
        _authServices.Register(register);
        return Ok("User created successfully");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("GetAllUsers")]
    public IActionResult GetAllUsers()
    {
      var users = _authServices.GetAllUsers();

      return Ok(users);
    }

    [HttpPost("LogIn")]
    public IActionResult LogIn(LoginRequest login)
    {

      // 1️⃣ Check email 
      if (string.IsNullOrWhiteSpace(login.Email))
        return BadRequest("Email is Requird");

      // 2️⃣ Check password
      if (string.IsNullOrWhiteSpace(login.Password))
        return BadRequest("Password is Requird");

      var LogIn = _authServices.LogIn(login);

      if (LogIn)
        return Ok("Login Success");

      return BadRequest("Email or password is incorrect");
    }

    [HttpPost("ChangePassword")]
    public IActionResult ChangePassword(ChangePassword changePassword)
    {
      // 1️⃣ Check email 
      if (string.IsNullOrWhiteSpace(changePassword.Email))
        return BadRequest("Email is Required");

      // 2️⃣ Check password
      if (string.IsNullOrWhiteSpace(changePassword.OldPassword))
        return BadRequest("Password is Required");

      // 2️⃣ Check password old pass
      if (string.IsNullOrWhiteSpace(changePassword.NewPassword))
        return BadRequest("New Password is Required");
      if (!IsValidPassword(changePassword.NewPassword))
        return BadRequest("Invalid Password format");


      if (_authServices.ChangePassword(changePassword))
        return Ok("Password changed successfully");


      return Unauthorized("Email or password is incorrect");
    }

  }
}