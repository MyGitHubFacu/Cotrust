using Cotrust.Intefaces;
using Cotrust.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class HomeController : Controller
    {
        #region Services

        private readonly IEmailSender _emailsender;
        private readonly CotrustDbContext _context;
        public HomeController(CotrustDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailsender = emailSender;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(int? Id = 1)
        {
            await UploadCart();

            switch (Id)
            {
                case 1: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.PLC).ToListAsync());
                case 2: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Module).ToListAsync());
                case 3: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.HMI).ToListAsync());
                case 4: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Servo).ToListAsync());
                case 5: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Driver).ToListAsync());
                case 6: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Software).ToListAsync());
                case 7: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Kits).ToListAsync());
                default: return View(await _context.Product.ToListAsync());
            }
            //if (_context.Product != null)
            //{
            //    await UploadCart();

            //    switch (Id)
            //    {
            //        case 1: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.PLC).ToListAsync());
            //        case 2: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Module).ToListAsync());
            //        case 3: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.HMI).ToListAsync());
            //        case 4: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Servo).ToListAsync());
            //        case 5: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Driver).ToListAsync());
            //        case 6: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Software).ToListAsync());
            //        case 7: return View(await _context.Product.Where(x => x.Kind == Product.TypeOfProduct.Kits).ToListAsync());
            //        default: return View(await _context.Product.ToListAsync());
            //    }
            //}
            //else
            //{
            //    return Problem("Entity set 'CotrustDbContext.Product'  is null.");
            //}                      
        }

        #endregion

        #region Help

        public async Task<IActionResult> Help()
        {
            await UploadCart();
            return View();
        }

        #endregion

        #region About

        public async Task<IActionResult> About()
        {
            await UploadCart();
            return View();
        }

        #endregion

        #region Contact

        public async Task<IActionResult> Contact()
        {
            await UploadCart();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Contact(string email, string body)
        {
            await _emailsender.SendEmailAsync(email, "Consulta", body);
            ViewBag.Message = "Enviado exitosamente";
            return View();
        }

        #endregion

        #region Others

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}