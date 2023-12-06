﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cotrust.Models;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class DirectionsController : Controller
    {
        #region Context

        private readonly CotrustDbContext _context;

        public DirectionsController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Directions).FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null) { return View(user.Directions); }
            }
            return RedirectToAction("AccessDenied", "User");                                                         
        }

        #endregion

        #region Details

        public async Task<IActionResult> Details(int? id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Directions).FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null)
                {
                    if (id == null || _context.Directions == null) { return NotFound(); }
                    var direction = await _context.Directions.FirstOrDefaultAsync(m => m.Id == id);
                    if (direction == null) { return NotFound(); }
                    return View(direction);
                }
            }
            return RedirectToAction("AccessDenied", "User");
        }

        #endregion

        #region Create

        public async Task<IActionResult> Create()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null) { return View(); }
            }
            return RedirectToAction("AccessDenied", "User");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Address,City,Region,Country,PostalCode")] Direction direction)
        {
            if (ModelState.IsValid)
            { 
                direction.UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _context.Add(direction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(direction);
        }

        #endregion

        #region Edit

        public async Task<IActionResult> Edit(int? id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Directions).FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null)
                {
                    if (id == null || _context.Directions == null) { return NotFound(); }
                    var direction = await _context.Directions.FindAsync(id);
                    if (direction == null) { return NotFound(); }
                    return View(direction);
                }
            }
            return RedirectToAction("AccessDenied", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Address,City,Region,Country,PostalCode")] Direction direction)
        {
            if (id != direction.Id) { return NotFound(); }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(direction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DirectionExists(direction.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(direction);
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Directions).FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null)
                {
                    if (id == null || _context.Directions == null) { return NotFound(); }
                    var direction = await _context.Directions.FirstOrDefaultAsync(m => m.Id == id);
                    if (direction == null) { return NotFound(); }
                    return View(direction);
                }
            }
            return RedirectToAction("AccessDenied", "User");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Directions == null) { return Problem("Entity set 'CotrustDbContext.Directions'  is null."); }
            var direction = await _context.Directions.FindAsync(id);
            if (direction != null) { _context.Directions.Remove(direction); }         
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Others

        private bool DirectionExists(int id)
        {
            return (_context.Directions?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        #endregion
    }
}
