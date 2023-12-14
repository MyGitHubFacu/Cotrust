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
    public class ProductsController : BaseController
    {
        #region Context

        public ProductsController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(Product.TypeOfProduct? kind)
        {
            try
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

                            List<SelectListItem> List = new List<SelectListItem>
                            {
                                new SelectListItem(Product.TypeOfProduct.PLC.ToString(), Product.TypeOfProduct.PLC.ToString(), false),
                                new SelectListItem(Product.TypeOfProduct.Module.ToString(), Product.TypeOfProduct.Module.ToString(), false),
                                new SelectListItem(Product.TypeOfProduct.HMI.ToString(), Product.TypeOfProduct.HMI.ToString(), false),
                                new SelectListItem(Product.TypeOfProduct.Servo.ToString(), Product.TypeOfProduct.Servo.ToString(), false),
                                new SelectListItem(Product.TypeOfProduct.Driver.ToString(), Product.TypeOfProduct.Driver.ToString(), false),
                                new SelectListItem(Product.TypeOfProduct.Software.ToString(), Product.TypeOfProduct.Software.ToString(), false),
                                new SelectListItem(Product.TypeOfProduct.Kits.ToString(), Product.TypeOfProduct.Kits.ToString(), false)
                            };

                            ViewBag.Kinds = List;

                            if (kind != null)
                            {
                                List.First(x => x.Text == kind.ToString()).Selected = true;
                                
                                return View(await _context.Product.Where(x => x.Kind == kind).ToListAsync());
                            }
                            else { return View(await _context.Product.ToListAsync()); }           
                        }
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
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
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
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
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Stock,Price,Kind,Image,File")] Product product)
        {
            try
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
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Stock,Price,Kind,Image,File")] Product product)
        {
            try
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
                    return RedirectToAction(nameof(Index), new { kind = product.Kind });
                }
                return View(product);
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
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
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Product == null) { return Problem("Entity set 'CotrustDbContext.Product'  is null."); }
                var product = await _context.Product.FindAsync(id);
                if (product != null) { _context.Product.Remove(product); }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { kind = product.Kind });
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
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
