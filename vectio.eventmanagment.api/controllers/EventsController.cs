using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vectio.eventmanagement.api.db;
using vectio.eventmanagement.api.db.entities;
using vectio.eventmanagement.api.helpers;

namespace vectio.eventmanagement.api.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventsController : ExtendedController
    {
        private readonly EventManagementDBContext _context;
        private readonly EmailHelper _emailHelper;

        public EventsController(EventManagementDBContext context, EmailHelper emailHelper)
        {
            _context = context;
            this._emailHelper = emailHelper;
        }

        // GET: api/Events
        [HttpGet]
        [Route("events")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> GetEvents()
        {
            var list = await JsonAsync(_context.JsonQueries.FromSqlInterpolated($"select * from dbo.getEventsForEdit(null) "));
            return list;
        }


        [HttpGet("{id}")]
        [Route("event")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var @event = await JsonAsync(_context.JsonQueries.FromSqlInterpolated($"select * from dbo.getEventsForEdit({id}) "));

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> PutEvent(Guid id, Event @event)
        {
            if (id != @event.Id)
            {
                return BadRequest();
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "administrator")]
        public async Task<ActionResult<Event>> PostEvent(Event @event)
        {
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrator")]
        public async Task<ActionResult<Event>> DeleteEvent(Guid id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return @event;
        }



        [HttpGet("/api/eventusers/{id}")]
        [Authorize(Roles = "administrator")]
        public async Task<ActionResult<IEnumerable<EventUser>>> GetEventUsers(Guid id)
        {
            return await _context.EventUsers.Where(e => e.EventId == id).ToListAsync();
        }


        [HttpGet("/api/reg/{id}")]

        [AllowAnonymous]
        public async Task<IActionResult> GetRegistration(Guid id)
        {



            var reg = await JsonAsync(_context.JsonQueries.FromSqlInterpolated($"select * from dbo.getEventsForEdit({id}) "));


            return reg;

        }

        [HttpPost]
        [Route("/api/sendregistration")]
        [AllowAnonymous]
        public async Task<ActionResult<EventUser>> SendRegistration(EventUser model)
        {
            var registration = new EventUser
            {
                EventId = model.EventId,
                CompanyName = model.CompanyName,
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Email = model.Email,
                Phone = model.Phone,
            };

            _context.EventUsers.Add(registration);
            await _context.SaveChangesAsync();

            var events = await _context.Events.FindAsync(model.EventId);

            var userMail = _emailHelper.SendEmail(registration.Email, "[VBP EMBA] - Potwierdzenie rejestracji"
                , string.Format(
                    @"
                    Szanowni Państwo,
                    <br/><br/>
                    Miło nam potwierdzić Państwa rejestrację na wydarzenie:<br/><br/>
                    {1}.<br/><br/>
                    Data: {2}
                    <br/><br/>
                    Opis wydarzenia: {3}                    
                    <br/> "
                    , "VBP - EMBA", events.EventName, events.EventDate, events.Content));



            //var adminMail = _emailHelper.SendEmail(registration.Email, "[VBP EMBA] - Potwierdzenie rejestracji"
            //    , string.Format(
            //        @"
            //        Szanowni Państwo,
            //        <br/><br/>
            //        Nowy uczestnik wydarzenia:<br/><br/>
            //        {1}<br/><br/>
            //        Data: {2}
            //        <br/><br/>
            //       Uczestnik: {3} {4}                
            //        <br/> "
            //        , "VBP - EMBA", events.EventName, events.EventDate, events.Content,registration.Firstname, registration.Lastname));



            return Ok(new { message = "Registration confirmation sent" });
        }


        private bool EventExists(Guid id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
