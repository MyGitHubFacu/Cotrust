using Cotrust.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Diagnostics;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class CartController : Controller
    {
        #region Context

        private readonly CotrustDbContext _context;

        public CartController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index()
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);
                    if (user != null)
                    {
                        List<CartProduct> lcp = await _context.CartProducts.Where(x => x.UserId == ident).Include(x => x.Product).ToListAsync();
                        ViewData["Products"] = lcp.Count();

                        double Total = 0;
                        foreach (CartProduct cp in lcp)
                        {
                            Total += cp.Product.Price * cp.Quantity;
                        }

                        ViewData["Total"] = Total;

                        return View(lcp);
                    }
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region AddProduct

        public async Task<IActionResult> AddProduct(int id, int quantity)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null)
                    {
                        Product? product = await _context.Product.FirstOrDefaultAsync(x => x.Id == id);

                        if (product != null)
                        {
                            CartProduct? cp = await _context.CartProducts.FirstOrDefaultAsync(x => x.UserId == ident & x.ProductId == id);

                            if (cp != null) /*Si ya existe el producto en el carrito le sumo la cantidad*/
                            {
                                cp.Quantity += quantity;
                                _context.Update(cp);
                            }
                            else /*Si no existe el producto en el carrito lo genero*/
                            {
                                CartProduct cartp = new CartProduct
                                {
                                    UserId = ident,
                                    ProductId = id,
                                    Quantity = quantity
                                };

                                _context.Add(cartp);
                            }

                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index)); ;
                        }
                    }
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        #endregion

        #region DeleteProduct 

        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                if (_context.CartProducts == null) { return Problem("Entity set 'CotrustDbContext.CartProducts'  is null."); }

                var product = await _context.CartProducts.FindAsync(id);
                if (product != null) { _context.CartProducts.Remove(product); }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
