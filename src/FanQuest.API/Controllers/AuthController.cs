using FanQuest.API.Contract;
using FanQuest.API.Extensions;
using FanQuest.API.Services;
using FanQuest.Application.Interfaces.Repositories;
using FanQuest.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FanQuest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly JwtTokenService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IUserRepository userRepo,
            JwtTokenService jwtService,
            ILogger<AuthController> logger)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] Login_Request request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return BadRequest(new { error = "Phone number is required" });

            _logger.LogInformation("Login attempt for phone: {Phone}", request.PhoneNumber);

            var user = await _userRepo.GetByPhoneNumberAsync(request.PhoneNumber);

            if (user == null)
            {
                // Auto-register new user
                var displayName = request.DisplayName ?? $"User_{request.PhoneNumber.Substring(request.PhoneNumber.Length - 4)}";
                user = new User(request.PhoneNumber, displayName);
                await _userRepo.CreateAsync(user);
                _logger.LogInformation("New user created: {UserId}", user.Id);
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user.Id, user.PhoneNumber, user.DisplayName);

            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    phoneNumber = user.PhoneNumber,
                    displayName = user.DisplayName,
                    totalPoints = user.TotalPoints
                }
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.GetUserId(); // Uses extension method
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { error = "User not found" });

            return Ok(new
            {
                id = user.Id,
                phoneNumber = user.PhoneNumber,
                displayName = user.DisplayName,
                totalPoints = user.TotalPoints
            });
        }

        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken()
        {
            var userId = User.GetUserId();
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                return NotFound(new { error = "User not found" });

            var newToken = _jwtService.GenerateToken(user.Id, user.PhoneNumber, user.DisplayName);

            return Ok(new { token = newToken });
        }
    }
}
