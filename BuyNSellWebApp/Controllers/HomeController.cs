using BuyNSellWebApp.Data;
using BuyNSellWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BuyNSellWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _environment = env;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products
                .Include(j => j.SubType)
                .OrderByDescending(j => j.ProductID);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ProductBySubType(int? id)
        {
            var subtype = await _context.SubTypes.FindAsync(id);
            ViewData["SubTypeName"] = "None";
            if (subtype != null)
            {
                ViewData["SubTypeName"] = subtype.SubTypeName;
            }
            var applicationDbContext = _context.Products
                .Include(j => j.SubType)
                .Where(m => m.SubTypeID == id);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> ViewDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.SubType)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize]
        public IActionResult SendRequest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = _context.Products
                .FirstOrDefault(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["ProductID"] = product.ProductID;
            ViewData["ProductName"] = product.ProductName;
            ViewData["UserName"] = product.UserName;
            ViewData["Price"] = product.Price;
            ViewData["ExtName"] = product.ExtName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest([Bind("RequestID,Price,Address,ContactNo,ProductID")] Request request)
        {
            ModelState.Remove("UserName");
            ModelState.Remove("Status");
            ModelState.Remove("RequestDate");
            if (ModelState.IsValid)
            {
                request.UserName = _userManager.GetUserName(this.User);
                request.RequestDate = DateTime.Now;
                request.Status = "Pending";
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyRequests));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "Description", request.ProductID);
            return View(request);
        }

        [Authorize]
        public async Task<IActionResult> MyRequests()
        {
            string userid = _userManager.GetUserName(this.User);
            var orders = _context.Requests
                .Include(m => m.Product)
                .Where(m => m.UserName == userid);
            return View(await orders.OrderByDescending(m => m.RequestID).ToListAsync());
        }

        [Authorize]
        public IActionResult SellProduct()
        {
            ViewData["SubTypeID"] = new SelectList(_context.SubTypes, "SubTypeID", "SubTypeName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellProduct([Bind("ProductID,ProductName,FileUpload,Description,Price,SubTypeID")] Product product)
        {
            ModelState.Remove("UserName");
            using (var memoryStream = new MemoryStream())
            {
                await product.FileUpload.FormFile.CopyToAsync(memoryStream);

                string photoname = product.FileUpload.FormFile.FileName;
                product.ExtName = Path.GetExtension(photoname);
                if (!".jpg.jpeg.png.gif.bmp".Contains(product.ExtName.ToLower()))
                {
                    ModelState.AddModelError("FileUpload.FormFile", "Invalid Format of Image Given.");
                }
                else
                {
                    ModelState.Remove("ExtName");
                }
            }
            if (ModelState.IsValid)
            {
                product.UserName = _userManager.GetUserName(this.User);
                _context.Add(product);
                await _context.SaveChangesAsync();
                var uploadsRootFolder = Path.Combine(_environment.WebRootPath, "photos");
                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }
                string filename = product.ProductID + product.ExtName;
                var filePath = Path.Combine(uploadsRootFolder, filename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await product.FileUpload.FormFile.CopyToAsync(fileStream).ConfigureAwait(false);
                }
                return RedirectToAction(nameof(MyProducts));
            }
            ViewData["SubTypeID"] = new SelectList(_context.SubTypes, "SubTypeID", "SubTypeName", product.SubTypeID);
            return View(product);
        }

        [Authorize]
        public async Task<IActionResult> MyProducts()
        {
            string userid = _userManager.GetUserName(this.User);
            var applicationDbContext = _context.Products.Include(p => p.SubType).Where(p => p.UserName == userid);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.SubType)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyProducts));
        }

        [Authorize]
        public async Task<IActionResult> ViewRequests(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.SubType)
                .Include(p => p.Requests)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.Requests
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (request == null)
            {
                return NotFound();
            }
            request.Status = "Approve";
            try
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
            }
            return RedirectToAction(nameof(MyProducts));
        }

        [Authorize]
        public async Task<IActionResult> DeclineRequest(int id)
        {
            var request = await _context.Requests
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (request == null)
            {
                return NotFound();
            }
            request.Status = "Decline";
            try
            {
                _context.Update(request);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

            }
            return RedirectToAction(nameof(MyProducts));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
