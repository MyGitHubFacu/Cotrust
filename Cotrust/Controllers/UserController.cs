using Cotrust.Intefaces;
using Cotrust.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class UserController : BaseController
    {
        #region Services

        private readonly IUtilities _utilities;
        private readonly IEmailSender _emailsender;

        public UserController(CotrustDbContext context, IUtilities utilities, IEmailSender emailsender)
        {
            _context = context;
            _utilities = utilities;
            _emailsender = emailsender;
        }

        #endregion

        #region Login

        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.HasMessage = false;
                return View();
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email == email && x.Password == _utilities.Encrypting(password));
               
                if (user != null)
                {
                    if (user.EmailConfirmed)
                    {
                        var Identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                        Identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                        Identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                        Identity.AddClaim(new Claim(ClaimTypes.Role, user.Type.ToString()));

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(Identity));

                        return RedirectToAction("Index", "Home");
                    }

                    ViewBag.HasMessage = true;
                    ViewBag.Message = "Confirmar e-mail para poder ingresar";
                    return View();
                }

                ViewBag.HasMessage = true;
                ViewBag.Message = "Usuario no encontrado";
                return View();
            }   
            catch (Exception ex)
            {
                return await HandleError(ex.Message); ; 
            }
        }

        #endregion

        #region Register

        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }         
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

                        user.Password = _utilities.Encrypting(user.Password2);
                        user.Token = _utilities.GenerateToken();

                        string? url = Url.ActionLink("EmailConfirmed", "User", new { token = user.Token });
                        await _emailsender.SendEmailAsync(user.Email, "Confirmar correo", "Click aquí: " + url);

                        _context.Add(user);
                        await _context.SaveChangesAsync();
                   
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
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Logout

        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region AccessDenied

        [AllowAnonymous]
        public async Task<IActionResult> AccessDenied()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }      
        }

        #endregion

        #region Account

        public async Task<IActionResult> Account()
        {
            try
            {
                await UploadCart();

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
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
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
                        user.Password = _utilities.Encrypting(user.Password2);

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
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Email confirmation

        [AllowAnonymous]
        public async Task<IActionResult> EmailConfirmed(string token)
        {
            try
            {
                User? user = await _context.User.FirstOrDefaultAsync(x => x.Token == token);

                if (user != null)
                {
                    user.EmailConfirmed = true;
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    var Identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    Identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                    Identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                    Identity.AddClaim(new Claim(ClaimTypes.Role, user.Type.ToString()));

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(Identity));

                    await UploadCart();

                    ViewBag.Message = "E-mail confirmado con exito";
                    return View();
                }

                ViewBag.Message = "No se pudo confirmar el E-mail";
                return View();
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion
    }
}
