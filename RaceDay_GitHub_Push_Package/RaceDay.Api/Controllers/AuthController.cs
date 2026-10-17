using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore; using RaceDay.Api.Data; using RaceDay.Api.DTOs; using RaceDay.Api.Models; using RaceDay.Api.Services; using Microsoft.AspNetCore.Authorization;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/auth")]
public class AuthController(RaceDayContext db,IPasswordService passwords,ITokenService tokens):ControllerBase {
 /// <summary>Register a participant account. Role selection is deliberately not accepted from the client.</summary>
 [HttpPost("register"),AllowAnonymous,ProducesResponseType(typeof(AuthResponse),201),ProducesResponseType(400)]
 public async Task<IActionResult> Register(RegisterRequest request){ var email=request.Email.Trim().ToLowerInvariant(); if(await db.Users.AnyAsync(x=>x.Email.ToLower()==email)) return Conflict(new{message="An account with this email already exists."}); var role=await db.Roles.SingleOrDefaultAsync(x=>x.RoleName=="Participant"); if(role is null)return Problem("Participant role is missing from the database.",statusCode:500); var u=new User{FirstName=request.FirstName.Trim(),LastName=request.LastName.Trim(),Email=email,RoleID=role.RoleID,Role=role,CreatedAt=DateTime.UtcNow};u.PasswordHash=passwords.Hash(u,request.Password);db.Users.Add(u);await db.SaveChangesAsync();var (token,exp)=tokens.Create(u,role.RoleName);return StatusCode(201,new AuthResponse(token,"Bearer",exp,u.ToResponse())); }
 /// <summary>Validate credentials and return a JWT access token.</summary>
 [HttpPost("login"),AllowAnonymous,ProducesResponseType(typeof(AuthResponse),200),ProducesResponseType(401)]
 public async Task<IActionResult> Login(LoginRequest request){var email=request.Email.Trim().ToLowerInvariant();var u=await db.Users.Include(x=>x.Role).SingleOrDefaultAsync(x=>x.Email.ToLower()==email);if(u is null||!passwords.Verify(u,u.PasswordHash,request.Password))return Unauthorized(new{message="Email or password is incorrect."});var(t,e)=tokens.Create(u,u.Role!.RoleName);return Ok(new AuthResponse(t,"Bearer",e,u.ToResponse()));}
 /// <summary>Return the current user's profile.</summary>
 [HttpGet("me"),Authorize,ProducesResponseType(typeof(UserResponse),200),ProducesResponseType(401)]
 public async Task<IActionResult> Me(){var u=await db.Users.Include(x=>x.Role).SingleOrDefaultAsync(x=>x.UserID==User.CurrentUserId());return u is null?NotFound():Ok(u.ToResponse());}
 /// <summary>Update the current user's basic profile. Role and password cannot be changed here.</summary>
 [HttpPut("me"),Authorize,ProducesResponseType(typeof(UserResponse),200),ProducesResponseType(409)]
 public async Task<IActionResult> UpdateMe(UpdateProfileRequest request){var id=User.CurrentUserId();var u=await db.Users.Include(x=>x.Role).SingleOrDefaultAsync(x=>x.UserID==id);if(u is null)return NotFound();var email=request.Email.Trim().ToLowerInvariant();if(await db.Users.AnyAsync(x=>x.UserID!=id&&x.Email.ToLower()==email))return Conflict(new{message="Email is already in use."});u.FirstName=request.FirstName.Trim();u.LastName=request.LastName.Trim();u.Email=email;await db.SaveChangesAsync();return Ok(u.ToResponse());}
 /// <summary>Change the current user's password after verifying the old password.</summary>
 [HttpPut("change-password"),Authorize,ProducesResponseType(204),ProducesResponseType(400)]
 public async Task<IActionResult> ChangePassword(ChangePasswordRequest request){var u=await db.Users.SingleOrDefaultAsync(x=>x.UserID==User.CurrentUserId());if(u is null)return NotFound();if(!passwords.Verify(u,u.PasswordHash,request.CurrentPassword))return BadRequest(new{message="Current password is incorrect."});u.PasswordHash=passwords.Hash(u,request.NewPassword);await db.SaveChangesAsync();return NoContent();}
}
