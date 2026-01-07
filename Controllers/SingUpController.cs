using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using App.Services;
using App.Models;

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

    [HttpPost("SingUp")]
    public IActionResult AddUser(SingUpModel user)
    {
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