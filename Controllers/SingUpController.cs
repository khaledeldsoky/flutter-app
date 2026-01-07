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
      if (Phone.Length != 11) return false;
      if (!Phone.All(char.IsDigit)) return false;
      string[] EgyptionPrefix = { "010", "011", "012", "015" };
      foreach (var item in Phone)
      {
        if (Phone.StartsWith(item))
        {
          return true;
        }
      }
      return false;
    }

    [HttpPost("SingUp")]
    public IActionResult AddUser(SingUpModel user)
    {


      if (!IsValidEmail(user.email))
        return BadRequest("Invalid email format");

      if (string.IsNullOrWhiteSpace(user.phone))
        return BadRequest("Phone is required");

      if (IsValidPhone(user.phone))
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
  }
}