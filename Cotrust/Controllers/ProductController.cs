using Cotrust.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class ProductController : Controller
    {
        #region Context

        private readonly CotrustDbContext _context;

        public ProductController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(int? id)
        {
            try
            {
                if (id == null || _context.Product == null) { return NotFound(); }
                var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
                if (product == null) { return NotFound(); }
                ViewData["RelatedProducts"] = await _context.Product.Where(x => x.Kind == product.Kind).ToListAsync();
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion
    }
}