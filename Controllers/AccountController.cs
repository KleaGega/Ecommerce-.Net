using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCProject.Data;
using MVCProject.Models;
using MVCProject.Services;
using MVCProject.ViewModels;
using SQLitePCL;
using System.Security.Claims;

namespace MVCProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly JwtService jwtService;
        private readonly IConfiguration configuration;
        private readonly MvcProductContext _context;


        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, JwtService jwtService, IConfiguration configuration, MvcProductContext context)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.configuration = configuration;
            _context = context;
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
                UserName = model.Email,
                City = model.City,
                PhoneNumber = model.PhoneNumber2,
                
            };

            var result = await userManager.CreateAsync(user, model.Password);
            await userManager.AddToRoleAsync(user, "User");

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

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid login attempt" });

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid login attempt" });

            var tokens = await jwtService.GenerateTokensAsync(user);

            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var expiresIn = int.Parse(configuration["Jwt:TokenValidityMins"]) * 60;

            return Ok(new
            {
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresIn = expiresIn,
                UserName = user.UserName,
                UserId = user.Id,
            });
        }

        // POST: api/Account/RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid refresh token" });
            }

            var tokens = await jwtService.GenerateTokensAsync(user);

            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await userManager.UpdateAsync(user);

            var expiresIn = int.Parse(configuration["Jwt:TokenValidityMins"]) * 60;

            return Ok(new
            {
                Token = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresIn = expiresIn,
                UserName = user.UserName
            });
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

        [Authorize]
        [HttpGet("WhoAmI")]
        public IActionResult WhoAmI()
        {
            var userName = User.Identity?.Name ?? "anonymous";
            var roles = User.Claims
                            .Where(c => c.Type == ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();

            return Ok(new { userName, roles });
        }

        [HttpGet("userInfoById/{userId}")]
        public async Task<IActionResult> GetUserInfoById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "UserId is required" });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.PhoneNumber,
                user.City,
            });
        }
    }

    }
