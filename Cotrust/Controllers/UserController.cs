using Cotrust.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class UserController : Controller
    {
        #region Context

        private readonly CotrustDbContext _context;

        public UserController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Login

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
                
                if (user != null)
                {
                    var Identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    Identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                    Identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));                  
                    Identity.AddClaim(new Claim(ClaimTypes.Role, user.Type.ToString()));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(Identity));

                    return RedirectToAction("Index", "Home");
                }

                /*Ver como mostrar el error*/
                ModelState.AddModelError("", "Usuario no encontrado.");
                return View();
            }   
            catch
            {
                return View();
            }
        }

        #endregion

        #region Register

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                if (user != null)
                {
                    if (user.Password == user.Password2)
                    {
                        if (await _context.User.AnyAsync(x => x.Email == user.Email))
                        {
                            ModelState.AddModelError(nameof(user.Email), "Ya hay una cuenta con ese e-mail");
                            return View(user);
                        }
                        user.Type = Models.User.TypeOfUser.Customer;

                        _context.Add(user);
                        await _context.SaveChangesAsync();

                        var Identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        Identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                        Identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                        Identity.AddClaim(new Claim(ClaimTypes.Role, user.Type.ToString()));

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(Identity));

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(user.Password), "Las contraseñas no coinciden");
                        return View(user);
                    }
                }
                return View(user);
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Logout

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        #endregion

        #region AccessDenied

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion

        #region Account

        public async Task<IActionResult> Account()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null)
                {
                    return View(user);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Account([Bind("Id,Name,Email,Password,Password2,Type,EmailConfirmed")] User user)
        {
            try
            {
                if (user != null)
                {
                    if (user.Password == user.Password2)
                    {
                        _context.Update(user);
                        await _context.SaveChangesAsync();

                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                        var Identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                        Identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                        Identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                        Identity.AddClaim(new Claim(ClaimTypes.Role, user.Type.ToString()));

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(Identity));

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(user.Password), "Las contraseñas no coinciden");
                        return View(user);
                    }
                }
                return View(user);
            }
            catch
            {
                return View();
            }
        }

        #endregion
    }
}
