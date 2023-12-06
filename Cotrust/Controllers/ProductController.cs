using Cotrust.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
            if (id == null || _context.Product == null) { return NotFound(); }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) { return NotFound(); }

            ViewData["RelatedProducts"] = await _context.Product.Where(x => x.Kind == product.Kind).ToListAsync();

            return View(product);
        }

        #endregion

        #region Error

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}