using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeningerDemoProject.Dtos.Account;
using WeningerDemoProject.Dtos.Invitation;
using WeningerDemoProject.Interfaces;
using WeningerDemoProject.Models;
using WeningerDemoProject.Validators;

namespace WeningerDemoProject.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IInvitationRepository _invitationRepo;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailSender emailSender,
            IInvitationRepository invitationRepo,
            ITokenService tokenService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _invitationRepo = invitationRepo;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ValidateModel]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var userName = loginDto.UserName?.Trim();
            var password = loginDto.Password?.Trim();

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                return BadRequest("Username & Password must not be empty!");

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());

            if (user == null)
                return Unauthorized("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Password check failed for user: {UserName}. IsLockedOut: {LockedOut}, " +
                    "IsNotAllowed: {NotAllowed}, RequiresTwoFactor: {Requires2FA}",
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

        [HttpPost("invite")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> InviteUser([FromBody] CreateInvitationDto dto)
        {
            var invitation = await _invitationRepo.CreateAsync(dto);

            if (invitation == null || invitation.IsUsed == true || invitation.ExpiresAt < DateTime.UtcNow) 
                return BadRequest("Invalid or expired invitation token.");

            var scheme = Request?.Scheme ?? "https";
            var host = Request?.Host.HasValue == true ? Request.Host.Value : "example.com";
            Console.WriteLine($"[InviteUser] Scheme: {scheme}, Host: {host}");

            var regLink = $"{scheme}://{host}/register?token={invitation.Id}";
            _logger.LogInformation($"Registration link created: {regLink}");

            await _emailSender.SendInvitationAsync(dto.Email, regLink);
            return Ok(new { invitation.Id, Link = regLink, ExpiresAt = invitation.ExpiresAt });
        }

        [HttpPost("register-user")]
        [ValidateModel]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto dto, Guid Id)
        {
            var invitation = await _invitationRepo.GetByIdAsync(dto.Token);
            if (invitation == null || invitation.IsUsed == true || invitation.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Invalid or expired invitation token.");

            try
            {
                var appUser = new AppUser
                {
                    UserName = dto.UserName,
                    Email = invitation.Email
                };

                var result = await _userManager.CreateAsync(appUser, dto.Password);

                if (!result.Succeeded)
                {
                    _logger.LogError("User creation failed for {UserName}. Errors: {Errors}", dto.UserName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));

                    return StatusCode(500, "User creation failed. Please try again later.");
                }

                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Role assignment failed for user: {UserName}. Error: {Errors}", appUser.UserName,
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                    return StatusCode(500, "Role assignment failed. Please try again later.");
                }

                await _invitationRepo.MarkUsedAsync(invitation);

                var token = _tokenService.CreateToken(appUser);

                return Ok(
                    new NewUserDto
                    {
                        UserName = appUser.UserName,
                        Email = appUser.Email,
                        Token = token,
                        Roles = await _userManager.GetRolesAsync(appUser)
                    }
                );

                //if (result.Succeeded)
                //{
                //    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                //    if (roleResult.Succeeded)
                //    {
                //        return Ok(
                //            new NewUserDto
                //            {
                //                UserName = appUser.UserName,
                //                Email = appUser.Email,
                //                Token = _tokenService.CreateToken(appUser),
                //                Roles = await _userManager.GetRolesAsync(appUser)
                //            }
                //        );
                //    }
                //    else
                //    {
                //        _logger.LogError("Role assignment failed for user: {UserName}. Error: {Errors}", appUser.UserName,
                //            string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                //        return StatusCode(500, "Role assignment failed. Please try again later.");
                //    }
                //}
                //else
                //{
                //    _logger.LogError("User creation failed for {UserName}. Errors: {Errors}", dto.UserName, 
                //        string.Join(", ", result.Errors.Select(e => e.Description)));

                //    return StatusCode(500, "User creation failed. Please try again later.");
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured during registration");
                return StatusCode(500, "Internal error");
            }
        }

        [HttpPost("register-admin")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto dto)
        {
            try
            {
                var appUser = new AppUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, dto.Password);

                if (!createdUser.Succeeded)
                {
                    _logger.LogError("Admin creation failed for {UserName}. Errors: {Errors}", dto.UserName,
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

        [HttpPut("update-user/{id}")]
        [ValidateModel]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound(string.Format("User {0} not found", id));

            var dtoUserName = dto.UserName?.Trim();
            var dtoEmail = dto.Email?.Trim();

            var userByName = dtoUserName != null
                ? await _userManager.FindByNameAsync(dtoUserName)
                : null;

            var userByEmail = dtoEmail != null
                ? await _userManager.FindByEmailAsync(dtoEmail)
                : null;

            if (dtoUserName != null)
            {
                if (userByName != null && userByName.Id != id)
                    return BadRequest($"Username {dtoUserName} is already taken.");
                
                user.UserName = dtoUserName;
            }
            
            if (dtoEmail != null)
            {
                if (userByEmail?.Email != null && userByEmail.Id != id)
                    return BadRequest($"Email '{dtoEmail}' is already taken");

                user.Email = dtoEmail;
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return StatusCode(500,
                    $"Failed to update user data: {string.Join("; ", updateResult.Errors.Select(e => e.Description))}");

            return NoContent();
            #region updateRoles
            //if (dto.Roles != null)
            //{
            //    var currentRoles = await _userManager.GetRolesAsync(user);

            //    var rolesToRemove = currentRoles.Except(dto.Roles);
            //    if (rolesToRemove.Any())
            //        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            //    var rolesToAdd = dto.Roles.Except(currentRoles);
            //    if (rolesToAdd.Any())
            //        await _userManager.AddToRolesAsync(user, rolesToAdd);
            //}
            #endregion
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
        [ValidateModel]
        [Authorize]
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
