using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VirginMaryCenter.Data;
using VirginMaryCenter.Models;

namespace VirginMaryCenter.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EventsController : Controller
    {
        private static readonly string[] AcceptableImages = new[]
        {
            "image/jpeg",
            "image/png"
        };

        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<EventsController> loc;

        public EventsController(ApplicationDbContext context, IStringLocalizer<EventsController> localization)
        {
            _context = context;
            loc = localization;
        }

        // GET: Admin/Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Admin/Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Admin/Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (@event != null && @event.Picture != null && !AcceptableImages.Contains(@event.Picture.ContentType?.ToLower()))
            {
                ModelState.AddModelError(nameof(Event.Picture), loc["Image must be JPG or PNG."].Value);
            }

            if (ModelState.IsValid)
            {
                using (var ms = new MemoryStream())
                {
                    await @event.Picture.CopyToAsync(ms);
                    @event.Image = ms.ToArray();
                }
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Admin/Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Admin/Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (@event != null && @event.Picture != null && !AcceptableImages.Contains(@event.Picture.ContentType?.ToLower()))
            {
                ModelState.AddModelError(nameof(Event.Picture), loc["Image must be JPG or PNG."].Value);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (@event.Picture != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await @event.Picture.CopyToAsync(ms);
                            @event.Image = ms.ToArray();
                        }
                    }
                    else
                    {
                        // no picture set, use the one from db
                        @event.Image = (await _context.Events.FindAsync(@event.Id)).Image;
                    }

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Admin/Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .SingleOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Admin/Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
