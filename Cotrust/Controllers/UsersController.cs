using System;
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
    public class UsersController : Controller
    {
        #region Context

        private readonly CotrustDbContext _context;

        public UsersController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(string search = "", User.TypeOfUser? type = null)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null && user.Type == Models.User.TypeOfUser.Admin)
                    {
                        if (type == null)
                        {
                            return View(await _context.User.Where(x => x.Name.Contains(search) | x.Email.Contains(search)).ToListAsync());
                        }
                        else
                        {
                            return View(await _context.User.Where(x => x.Type == type).ToListAsync());
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region Details

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null && user.Type == Models.User.TypeOfUser.Admin)
                    {
                        if (id == null || _context.User == null) { return NotFound(); }
                        var us = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
                        if (us == null) { return NotFound(); }
                        return View(us);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region Create

        public async Task<IActionResult> Create()
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null && user.Type == Models.User.TypeOfUser.Admin)
                    {
                        return View();
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }         
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Password,Type,EmailConfirmed")] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region Edit

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null && user.Type == Models.User.TypeOfUser.Admin)
                    {
                        if (id == null || _context.User == null) { return NotFound(); }
                        var us = await _context.User.FindAsync(id);
                        if (us == null) { return NotFound(); }
                        return View(us);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Password,Type,EmailConfirmed")] User user)
        {
            try
            {
                if (id != user.Id) { return NotFound(); }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UserExists(user.Id)) { return NotFound(); }
                        else { throw; }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null && user.Type == Models.User.TypeOfUser.Admin)
                    {
                        if (id == null || _context.User == null) { return NotFound(); }
                        var us = await _context.User.FirstOrDefaultAsync(m => m.Id == id);
                        if (us == null) { return NotFound(); }
                        return View(us);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.User == null) { return Problem("Entity set 'CotrustDbContext.User'  is null."); }
                var user = await _context.User.FindAsync(id);
                if (user != null) { _context.User.Remove(user); }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region Other

        private bool UserExists(int id)
        {
            return (_context.User?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        #endregion
    }
}
