using System.Security.Claims;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;
namespace RaceDay.Api.Services;
public static class ControllerHelpers { public static int CurrentUserId(this ClaimsPrincipal p)=>int.Parse(p.FindFirstValue(ClaimTypes.NameIdentifier)!); public static UserResponse ToResponse(this User u)=>new(u.UserID,u.FirstName,u.LastName,u.Email,u.Role?.RoleName??"Unknown",u.CreatedAt.ToString("O")); public static EventResponse ToResponse(this RaceEvent e)=>new(e.EventID,e.EventName,e.Description,e.EventDate,e.Location,e.Status,e.OrganiserID); public static CategoryResponse ToResponse(this Category c)=>new(c.CategoryID,c.CategoryName,c.DistanceKm,c.CreatedByUserID); }
