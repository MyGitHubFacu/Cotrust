using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cotrust.Models;
using Microsoft.CodeAnalysis.Host;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class ProductsController : Controller
    {
        #region Context

        private readonly CotrustDbContext _context;

        public ProductsController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(Product.TypeOfProduct? kind)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null)
                {
                    if (user.Type == Models.User.TypeOfUser.Admin | user.Type == Models.User.TypeOfUser.Staff)
                    {
                        if (_context.Product == null) { return Problem("Entity set 'CotrustDbContext.Product'  is null."); }
                        if (kind != null) { return View(await _context.Product.Where(x => x.Kind == kind).ToListAsync()); }
                        else { return View(await _context.Product.ToListAsync()); }
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Details

        public async Task<IActionResult> Details(int? id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null)
                {
                    if (user.Type == Models.User.TypeOfUser.Admin | user.Type == Models.User.TypeOfUser.Staff)
                    {
                        if (id == null || _context.Product == null) { return NotFound(); }
                        var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
                        if (product == null) { return NotFound(); }
                        return View(product);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Create

        public async Task<IActionResult> Create()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null)
                {
                    if (user.Type == Models.User.TypeOfUser.Admin | user.Type == Models.User.TypeOfUser.Staff)
                    {
                        return View();
                    }                        
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Stock,Price,Kind,Image,File")] Product product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (product.File != null)
                    {
                        Stream fs = product.File.OpenReadStream();
                        BinaryReader br = new BinaryReader(fs);
                        byte[] bytes = br.ReadBytes((int)fs.Length);
                        product.Image = Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
                catch { }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        #endregion

        #region Edit

        public async Task<IActionResult> Edit(int? id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null)
                {
                    if (user.Type == Models.User.TypeOfUser.Admin | user.Type == Models.User.TypeOfUser.Staff)
                    {
                        if (id == null || _context.Product == null) { return NotFound(); }
                        var product = await _context.Product.FindAsync(id);
                        if (product == null) { return NotFound(); }
                        return View(product);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Stock,Price,Kind,Image,File")] Product product)
        {
            if (id != product.Id) { return NotFound(); }

            if (ModelState.IsValid)
            {
                try
                {
                    try
                    {
                        if (product.File != null)
                        {
                            Stream fs = product.File.OpenReadStream();
                            BinaryReader br = new BinaryReader(fs);
                            byte[] bytes = br.ReadBytes((int)fs.Length);
                            product.Image = Convert.ToBase64String(bytes, 0, bytes.Length);
                        }
                    }
                    catch { }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null)
                {
                    if (user.Type == Models.User.TypeOfUser.Admin | user.Type == Models.User.TypeOfUser.Staff)
                    {
                        if (id == null || _context.Product == null) { return NotFound(); }
                        var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
                        if (product == null) { return NotFound(); }
                        return View(product);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null) { return Problem("Entity set 'CotrustDbContext.Product'  is null."); }
            var product = await _context.Product.FindAsync(id);
            if (product != null) { _context.Product.Remove(product); }           
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Others

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        #endregion
    }
}
