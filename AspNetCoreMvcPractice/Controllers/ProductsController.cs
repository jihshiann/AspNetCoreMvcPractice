using AspNetCoreMvcPractice.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMvcPractice.Controllers
{
    public class ProductsController : Controller
    {
        private readonly NorthwindContext _context;

        // Injected into the controller, dependency configured in Program.cs.
        public ProductsController(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Retrieve all products from the Products table using LINQ.
            var products = await _context.Products.ToListAsync();

            return View(products);
        }
    }
}