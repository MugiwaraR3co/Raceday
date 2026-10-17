using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using RaceDay.Api.Data;using RaceDay.Api.DTOs;using RaceDay.Api.Models;using RaceDay.Api.Services;
namespace RaceDay.Api.Controllers;
[ApiController,Route("api/events")]
public class EventsController(RaceDayContext db):ControllerBase{
 /// <summary>List events. Optional status filter: Open, Closed, Cancelled or Completed.</summary>
 [HttpGet,AllowAnonymous,ProducesResponseType(typeof(IEnumerable<EventResponse>),200)] public async Task<IActionResult> GetAll([FromQuery]string? status){var q=db.Events.AsNoTracking();if(!string.IsNullOrWhiteSpace(status))q=q.Where(x=>x.Status==status);return Ok(await q.OrderBy(x=>x.EventDate).Select(x=>new EventResponse(x.EventID,x.EventName,x.Description,x.EventDate,x.Location,x.Status,x.OrganiserID)).ToListAsync());}
 /// <summary>Get one event.</summary>
 [HttpGet("{id:int}"),AllowAnonymous,ProducesResponseType(typeof(EventResponse),200),ProducesResponseType(404)] public async Task<IActionResult> Get(int id){var e=await db.Events.AsNoTracking().SingleOrDefaultAsync(x=>x.EventID==id);return e is null?NotFound():Ok(e.ToResponse());}
 /// <summary>Create an event. The organiser is taken from the authenticated token.</summary>
 [HttpPost,Authorize(Roles="Organiser"),ProducesResponseType(typeof(EventResponse),201),ProducesResponseType(400)] public async Task<IActionResult> Create(EventRequest r){if(r.EventDate.Date<DateTime.UtcNow.Date)return BadRequest(new{message="Event date cannot be in the past."});var e=new RaceEvent{OrganiserID=User.CurrentUserId(),EventName=r.EventName.Trim(),Description=r.Description,EventDate=r.EventDate.Date,Location=r.Location.Trim(),Status="Open"};db.Events.Add(e);await db.SaveChangesAsync();return CreatedAtAction(nameof(Get),new{id=e.EventID},e.ToResponse());}
 /// <summary>Update an event. Organisers may update any event.</summary>
 [HttpPut("{id:int}"),Authorize(Roles="Organiser"),ProducesResponseType(typeof(EventResponse),200),ProducesResponseType(404)] public async Task<IActionResult> Update(int id,EventRequest r){var e=await db.Events.FindAsync(id);if(e is null)return NotFound();if(r.EventDate.Date<DateTime.UtcNow.Date)return BadRequest(new{message="Event date cannot be in the past."});e.EventName=r.EventName.Trim();e.Description=r.Description;e.EventDate=r.EventDate.Date;e.Location=r.Location.Trim();await db.SaveChangesAsync();return Ok(e.ToResponse());}
 /// <summary>Cancel an event rather than hard-deleting related enrolments.</summary>
 [HttpDelete("{id:int}"),Authorize(Roles="Organiser"),ProducesResponseType(204),ProducesResponseType(404)] public async Task<IActionResult> Cancel(int id){var e=await db.Events.FindAsync(id);if(e is null)return NotFound();e.Status="Cancelled";await db.SaveChangesAsync();return NoContent();}
}
