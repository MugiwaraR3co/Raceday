using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Api.Models;
namespace RaceDay.Api.Services;
public interface IPasswordService { string Hash(User user,string password); bool Verify(User user,string hash,string password); }
public class PasswordService : IPasswordService { private readonly PasswordHasher<User> _hasher=new(); public string Hash(User user,string password)=>_hasher.HashPassword(user,password); public bool Verify(User user,string hash,string password)=>_hasher.VerifyHashedPassword(user,hash,password)!=PasswordVerificationResult.Failed; }
public interface ITokenService { (string token,DateTime expires) Create(User user,string role); }
public class TokenService(IConfiguration config) : ITokenService { public (string token,DateTime expires) Create(User user,string role) { var s=config.GetSection("Jwt"); var expires=DateTime.UtcNow.AddMinutes(int.TryParse(s["Minutes"],out var m)?m:60); var claims=new[]{new Claim(ClaimTypes.NameIdentifier,user.UserID.ToString()),new Claim(ClaimTypes.Email,user.Email),new Claim(ClaimTypes.Name,$"{user.FirstName} {user.LastName}"),new Claim(ClaimTypes.Role,role)}; var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s["Key"]!)); var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha256); var jwt=new JwtSecurityToken(s["Issuer"],s["Audience"],claims,expires:expires,signingCredentials:creds); return(new JwtSecurityTokenHandler().WriteToken(jwt),expires); } }
