﻿using Cotrust.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Orders;
using System.Security.Claims;

namespace Cotrust.Controllers
{
    public class PackagesController : Controller
    {
        #region Context

        private readonly CotrustDbContext _context;

        public PackagesController(CotrustDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null)
                {
                    ViewData["Products"] = user.Products.Count;
                    return View(await _context.Package.Include(x => x.Products).Where(x => x.UserId == ident).ToListAsync());
                }
            }
            return RedirectToAction("AccessDenied", "User");
        }

        #endregion

        #region Products

        public async Task<IActionResult> Products(int id)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == ident);
                if (user != null)
                {
                    ViewData["Products"] = user.Products.Count;
                    Package P = await _context.Package.FirstAsync(x => x.Id == id);
                    ViewData["PackageName"] = P.Name;
                    return View(await _context.PackageProducts.Where(x => x.PackageId == id).Include(x => x.Product).ToListAsync());
                }
            }
            return RedirectToAction("AccessDenied", "User");
        }

        #endregion

        #region AddPackage

        public async Task<IActionResult> PackageName()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null && user.Products.Count > 0)
                {
                    return View();
                }
            }
            return RedirectToAction("Index", "Cart");       
        }

        public async Task<IActionResult> AddPackage(string name)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                int ident = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User? user = await _context.User.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == ident);

                if (user != null)
                {
                    if (_context.Package.FirstOrDefault(x => x.Name == name) == null)
                    {
                        Package pck = new Package();
                        pck.UserId = ident;
                        pck.Name = name;
                        _context.Add(pck);
                        await _context.SaveChangesAsync();

                        Package p = await _context.Package.FirstAsync(x => x.Name == pck.Name);

                        foreach (CartProduct cp in user.Products)
                        {
                            if (cp != null)
                            {
                                PackageProduct pp = new PackageProduct();
                                pp.ProductId = cp.ProductId;
                                pp.Quantity = cp.Quantity;
                                pp.PackageId = p.Id;

                                _context.Add(pp);
                            }
                        }
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["NameExist"] = "El nombre ya existe";
                    return RedirectToAction(nameof(PackageName));
                }
            }
            return RedirectToAction("AccessDenied", "User");
        }

        #endregion

        #region DeletePackage

        public async Task<IActionResult> DeletePackage(int id)
        {
            if (_context.Package == null) { return Problem("Entity set 'CotrustDbContext.CartProducts'  is null."); }

            var product = await _context.Package.FindAsync(id);
            if (product != null) { _context.Package.Remove(product); }

            List<PackageProduct> List = await _context.PackageProducts.Where(x => x.PackageId == id).ToListAsync();

            foreach (PackageProduct p in List)
            {
                _context.Remove(p);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region AddToCart

        public async Task<IActionResult> AddToCart(int id)
        {
            if (_context.Package == null) { return Problem("Entity set 'CotrustDbContext.CartProducts'  is null."); }

            var Package = await _context.Package.FindAsync(id);

            if (Package != null)
            {
                List<PackageProduct> List = await _context.PackageProducts.Where(x => x.PackageId == id).ToListAsync();

                foreach (PackageProduct pp in List)
                {
                    if (pp != null)
                    {
                        CartProduct bp = new CartProduct();
                        bp.UserId = Package.UserId;
                        bp.ProductId = pp.ProductId;
                        bp.Quantity = pp.Quantity;

                        _context.Add(bp);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Cart");
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
