using Cotrust.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class BuysController : BaseController
    {
        #region Context

        public BuysController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index()
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
                        if (_context.Buys == null) { return Problem("Entity set 'CotrustDbContext.Buys'  is null."); }

                        List<Buys> buys = await _context.Buys.Where(x => x.UserId == ident).OrderByDescending(x => x.Date).ToListAsync();

                        foreach (Buys b in user.Buys)
                        {
                            b.Products = await _context.BuysProducts.Where(x => x.BuysId == b.Id).Include(x => x.Product).ToListAsync();
                        }
                        return View(user.Buys);
                    }
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Products

        public async Task<IActionResult> Products(int id)
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
                        List<BuysProduct> list = await _context.BuysProducts.Where(x => x.BuysId == id).Include(x => x.Product).ToListAsync();
                        return View(list);
                    }
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Buy steps

        #region Step 1 - Select products

        public async Task<IActionResult> BuyProduct(int id)
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null)
                    {
                        CartProduct? cp = await _context.CartProducts.FirstOrDefaultAsync(x => x.Id == id);

                        if (cp != null)
                        {
                            TempData["UserId"] = user.Id;
                            TempData["CartProduct"] = id;

                            return RedirectToAction("ChooseDirection");
                        }
                    }
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        public async Task<IActionResult> BuyAll()
        {
            try
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == ident);
                    if (user != null && user.Products.Count > 0)
                    {
                        TempData["UserId"] = user.Id;
                        TempData["CartProduct"] = -1;

                        return RedirectToAction("ChooseDirection");
                    }
                    return RedirectToAction("Index", "Cart");
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Step 2 - Choose direction

        public async Task<IActionResult> ChooseDirection()
        {
            try
            {
                await UploadCart();

                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    User? user = await _context.User.Include(x => x.Directions).FirstOrDefaultAsync(x => x.Id == ident);

                    if (user != null)
                    {
                        if (TempData["UserId"] != null)
                        {
                            TempData.Keep();
                            return View(user.Directions.ToList());
                        }
                        return RedirectToAction("Index", "Cart");
                    }
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        public async Task<IActionResult> DirectionChoosed(int id)
        {
            try
            {
                Direction Dir = _context.Directions.First(x => x.Id == id);
                TempData["Direction"] = Dir.Address + ", " + Dir.City + ", " + Dir.Region + ", " + Dir.Country + " - " + Dir.PostalCode;
                TempData.Keep();
                return RedirectToAction("Buy");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #region Step 3 - Confirm buy

        public async Task<IActionResult> Buy()
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
                        TempData.Keep();
                        return View();
                    }
                    return RedirectToAction("Index", "Cart");
                }
                return RedirectToAction("AccessDenied", "User");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        public async Task<IActionResult> BuyNow()
        {
            try
            {
                int UserId = Convert.ToInt32(TempData["UserId"]);
                string? Direction = Convert.ToString(TempData["Direction"]);
                int CartProduct = Convert.ToInt32(TempData["CartProduct"]);

                if (UserId != 0)
                {
                    Buys buy = new Buys();
                    buy.UserId = UserId;
                    buy.Direction = Direction;
                    buy.Date = DateTime.Now;
                    _context.Add(buy);
                    await _context.SaveChangesAsync();

                    Buys b = await _context.Buys.FirstAsync(x => x.Date == buy.Date);

                    List<CartProduct> List = new List<CartProduct>();
                    if (CartProduct == -1) { List = await _context.CartProducts.Where(x => x.UserId == UserId).ToListAsync(); }
                    else { List.Add(await _context.CartProducts.FirstAsync(x => x.Id == CartProduct)); }

                    foreach (CartProduct cp in List)
                    {
                        if (cp != null)
                        {
                            BuysProduct bp = new BuysProduct();
                            bp.ProductId = cp.ProductId;
                            bp.Quantity = cp.Quantity;
                            bp.BuysId = b.Id;

                            _context.Add(bp);
                        }
                    }

                    foreach (CartProduct cp in List) { _context.Remove(cp); }

                    await _context.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                return await HandleError(ex.Message);
            }
        }

        #endregion

        #endregion
    }
}
