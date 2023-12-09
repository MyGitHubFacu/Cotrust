using Cotrust.Intefaces;
using Cotrust.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class BaseController : Controller
    {
        protected CotrustDbContext _context;

        public async Task<IActionResult> HandleError(string Message)
        {
            await UploadCart();
            TempData["Message"] = Message;
            return RedirectToAction("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Actualiza el número de productos en el carrito "_Layout"
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UploadCart()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null) { ViewData["Products"] = user.Products.Count(); }
            }
            return true;
        }
    }
}
