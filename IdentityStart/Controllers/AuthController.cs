using IdentityStart.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityStart.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp(SignUpModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = new IdentityUser
        {
            Email = model.EmailAddress,
            UserName = model.EmailAddress
        };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            result.Errors.ToList().ForEach(error => ModelState.AddModelError(error.Code, error.Description));
            return BadRequest(ModelState);
        }

        return Ok();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
            return Ok();

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return BadRequest();
    }

    [Authorize]
    [HttpPost("sign-out")]
    public async Task<IActionResult> SignOutUser()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}