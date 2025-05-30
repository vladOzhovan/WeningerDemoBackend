using Microsoft.AspNetCore.Mvc;
using WeningerDemoProject.Models;
using Microsoft.AspNetCore.Identity;
using WeningerDemoProject.Dtos.Account;
using WeningerDemoProject.Interfaces;
using Microsoft.EntityFrameworkCore;
using WeningerDemoProject.Validators;
using Microsoft.AspNetCore.Authorization;

namespace WeningerDemoProject.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("register-user")]
        [Authorize(Roles = "Admin")]
        [ValidateModel]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            try
            {
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                Token = _tokenService.CreateToken(appUser),
                                Roles = await _userManager.GetRolesAsync(appUser)
                            }
                        );
                    }
                    else
                    {
                        _logger.LogError("Role assignment failed for user: {UserName}. Error: {Errors}", appUser.UserName,
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                        return StatusCode(500, "Role assignment failed. Please try again later.");
                    }
                }
                else
                {
                    _logger.LogError("User creation failed for {UserName}. Errors: {Errors}", registerDto.UserName, 
                        string.Join(", ", createdUser.Errors.Select(e => e.Description)));

                    return StatusCode(500, "User creation failed. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured during registration");
                return StatusCode(500, "Internal error");
            }
        }

        [HttpPost("register-admin")]
        [Authorize(Roles = "Admin")]
        [ValidateModel]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto registerDto)
        {
            try
            {
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (!createdUser.Succeeded)
                {
                    _logger.LogError("Admin creation failed for {UserName}. Errors: {Errors}", registerDto.UserName,
                        string.Join(", ", createdUser.Errors.Select(e => e.Description)));
                    return StatusCode(500, "Admin creation failed. Try again later");
                }

                var roleResult = await _userManager.AddToRoleAsync(appUser, "Admin");

                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Admin role assignment failed for {UserName}. Errors: {Errors}", appUser.UserName,
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    return StatusCode(500, "Role assignment failed. Please try again later.");
                }

                return Ok(new NewUserDto
                {
                    UserName = appUser.UserName,
                    Email = appUser.Email,
                    Token = _tokenService.CreateToken(appUser)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured during admin registration");
                return StatusCode(500, "Internal error");
            }
        }

        [HttpPost("login")]
        [ValidateModel]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginDto.UserName.ToLower());

            _logger.LogInformation("Trying to log in user: {InputUserName}. Found in DB: {DbUserName}",
                loginDto.UserName, user?.UserName);

            if (user == null)
                return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Password check failed for user: {UserName}. IsLockedOut: {LockedOut}, IsNotAllowed: {NotAllowed}, RequiresTwoFactor: {Requires2FA}",
                    user.UserName, result.IsLockedOut, result.IsNotAllowed, result.RequiresTwoFactor);

                return Unauthorized("Invalid email or password");
            }

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user),
                    Roles = await _userManager.GetRolesAsync(user)
                }
            );
        }

        [HttpPut("update-user/{id}")]
        [Authorize(Roles = "Admin")]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound(string.Format("User {0} not found", id));

            if (dto.UserName != null)
            {
                var existingUser = await _userManager.FindByNameAsync(dto.UserName);
                if (existingUser != null && existingUser.Id != id)
                    return BadRequest($"Username {dto.UserName} id already taken.");
                
                user.UserName = dto.UserName;
            }
            
            if (dto.Email != null)
                user.Email = dto.Email;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return StatusCode(500,
                    $"Failed to update user data: {string.Join("; ", updateResult.Errors.Select(e => e.Description))}");

            if (dto.Roles != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                var rolesToRemove = currentRoles.Except(dto.Roles);
                if (rolesToRemove.Any())
                    await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

                var rolesToAdd = dto.Roles.Except(currentRoles);
                if (rolesToAdd.Any())
                    await _userManager.AddToRolesAsync(user, rolesToAdd);
            }

            return NoContent();
        }

        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound($"User {id} not found.");

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} deleted", user.UserName);
                return Ok($"User {user.UserName} deleted");  
            }
            else
            {
                _logger.LogError("Failed to delete user '{Username}'. Errors: {Errors}",
                    user.UserName, string.Join(", ", result.Errors.Select(e => e.Description)));

                return StatusCode(500, "Failed to delete user.");
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        [ValidateModel]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Unauthorized();

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest($"Failed to change password: {errors}");
            }

            _logger.LogInformation("User {UserName} changed password succesfully.", user.UserName);

            return NoContent();
        }

        [HttpGet("get-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                var userDtos = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles.ToList()
                    });
                }

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
