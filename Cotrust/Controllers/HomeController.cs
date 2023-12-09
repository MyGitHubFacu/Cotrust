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
    public class HomeController : BaseController
    {
        #region Services

        private readonly IEmailSender _emailsender;
        public HomeController(CotrustDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailsender = emailSender;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(int? Id = 1)
        {
            try
            {
                if (_context.Product == null) { return Problem("Entity set 'CotrustDbContext.Product'  is null."); }

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
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Help

        public async Task<IActionResult> Help()
        {
            try
            {
                await UploadCart();
                return View();
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region About

        public async Task<IActionResult> About()
        {
            try
            {
                await UploadCart();
                return View();
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Contact

        public async Task<IActionResult> Contact()
        {
            try
            {
                await UploadCart();
                return View();
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
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

        public IActionResult Privacy()
        {
            return View();
        }

        #endregion
    }
}