using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using App.Services;
using App.Models;
using System.Net.Mail;

namespace App.Controllers
{
  [ApiController]
  [Route("api/")]
  public class SingUpControllers : ControllerBase
  {
    private readonly SingUpServices _signUpServices;

    public SingUpControllers(SingUpServices signUpServices)
    {
      _signUpServices = signUpServices;
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


    [HttpPost("SingUp")]
    public IActionResult AddUser(SingUpModel user)
    {
      // 1️⃣ Check email 
      if (string.IsNullOrWhiteSpace(user.email))
        return BadRequest("Email is Requird");
      if (!IsValidEmail(user.email))
        return BadRequest("Invalid email format");

      // 2️⃣ Check password
      if (string.IsNullOrWhiteSpace(user.passowrd))
        return BadRequest("Password is Requird");
      if (!IsValidPassword(user.passowrd))
        return BadRequest("Invalid Password format");

      // 2️⃣ Check phone
      if (string.IsNullOrWhiteSpace(user.phone))
        return BadRequest("Phone is required");
      if (!IsValidPhone(user.phone))
        return BadRequest("Invalid phone format");

      try
      {
        _signUpServices.AddUser(user);
        return Ok("The user is add success");
      }
      catch (Exception ex)
      {
        return BadRequest(ex.Message);
      }
    }

    [HttpGet("GetAllUsers")]
    public IActionResult GetAllUsers()
    {
      var users = _signUpServices.GetAllUsers();
      return Ok(users);
    }

    [HttpPost("LogIn")]
    public IActionResult LogIn(string Email, string Password)
    {
      var SingIn = _signUpServices.LogIn(Email, Password);

      if (SingIn)
        return Ok("Login Success");
      
      return BadRequest("Email or password is incorrect");
    }
  }
}