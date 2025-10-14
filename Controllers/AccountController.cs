using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCProject.Models;
using MVCProject.ViewModels;

namespace MVCProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]  
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        // POST: api/Account/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new Users
            {
                FullName = model.Name,
                Email = model.Email,
                UserName = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok(new { message = "User registered successfully" });

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        // POST: api/Account/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
                return Ok(new { message = "Login successful" });

            return Unauthorized(new { message = "Invalid login attempt" });
        }

        // POST: api/Account/VerifyEmail
        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailView model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(model.Email);

            if (user == null)
                return NotFound(new { message = "Email not found" });

            return Ok(new { username = user.UserName });
        }

        // POST: api/Account/ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(model.Email);
            if (user == null)
                return NotFound(new { message = "User not found" });

            var removeResult = await userManager.RemovePasswordAsync(user);
            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors.Select(e => e.Description));

            var addResult = await userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addResult.Succeeded)
                return BadRequest(addResult.Errors.Select(e => e.Description));

            return Ok(new { message = "Password changed successfully" });
        }

        // POST: api/Account/Logout
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok(new { message = "Logged out" });
        }
    }
}
