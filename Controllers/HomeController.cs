using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCProject.Data;
using MVCProject.Models;
using System.Diagnostics;

namespace MVCProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly MvcProductContext _context;

        public HomeController(MvcProductContext context)
        {
            _context = context; 
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult OurTeam()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {         
            var products = await _context.Product
                                         .OrderByDescending(p => p.Id) 
                                         .Take(8)
                                         .ToListAsync();

            return View(products); 
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
