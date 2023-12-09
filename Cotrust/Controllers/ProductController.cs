using Cotrust.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class ProductController : BaseController
    {
        #region Context

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
                await UploadCart();

                if (id == null || _context.Product == null) { return NotFound(); }
                var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
                if (product == null) { return NotFound(); }
                ViewData["RelatedProducts"] = await _context.Product.Where(x => x.Kind == product.Kind).ToListAsync();
                return View(product);
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion
    }
}