using EpinHell.Data;
using EpinHell.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EpinHell.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to inject the database context and user manager
        public ProductController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Display the create product form (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")] // Restrict access to users with the "Admin" role
        public IActionResult Create()
        {
            return View();
        }

        // POST: Handle the creation of a new product (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent Cross-Site Request Forgery (CSRF)
        [Authorize(Roles = "Admin")] // Restrict access to users with the "Admin" role
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product); // Add the product to the database
                await _context.SaveChangesAsync(); // Save changes
                return RedirectToAction(nameof(Index)); // Redirect to the product listing
            }
            return View(product); // Return the form with validation errors if any
        }

        // GET: Display a list of all products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync(); // Retrieve all products
            return View(products); // Render the product list view
        }

        // GET: Display the edit product form (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")] // Restrict access to users with the "Admin" role
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Return 404 if the product ID is null
            }

            var product = await _context.Products.FindAsync(id); // Find the product by ID
            if (product == null)
            {
                return NotFound(); // Return 404 if the product is not found
            }
            return View(product); // Render the edit product form
        }

        // POST: Handle the edit form submission (Admin only)
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF
        [Authorize(Roles = "Admin")] // Restrict access to users with the "Admin" role
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,ImageUrl,CreatedAt")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound(); // Return 404 if the IDs do not match
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product); // Update the product in the database
                    await _context.SaveChangesAsync(); // Save changes
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound(); // Return 404 if the product no longer exists
                    }
                    else
                    {
                        throw; // Rethrow the exception if it's not a concurrency issue
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to the product listing
            }
            return View(product); // Return the form with validation errors if any
        }

        // POST: Handle the delete product action (Admin only)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Prevent CSRF
        [Authorize(Roles = "Admin")] // Restrict access to users with the "Admin" role
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id); // Find the product by ID
            if (product != null)
            {
                _context.Products.Remove(product); // Remove the product from the database
                await _context.SaveChangesAsync(); // Save changes
            }

            return RedirectToAction(nameof(Index)); // Redirect to the product listing
        }

        // Helper method to check if a product exists by its ID
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id); // Check for existence in the database
        }

        // POST: Add a product to the user's cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            var product = await _context.Products.FindAsync(productId); // Find the product by ID
            if (product == null)
            {
                return NotFound(); // Return 404 if the product is not found
            }

            var userId = _userManager.GetUserId(User); // Get the current user's ID
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if the user is not authenticated
            }

            // Retrieve the user's cart or create a new one if it doesn't exist
            var cart = await _context.Carts
                                      .Include(c => c.CartItems)
                                      .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Save the new cart
            }

            // Check if the product is already in the cart
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity++; // Increase the quantity if the product is already in the cart
            }
            else
            {
                // Add a new cart item for the product
                cart.CartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1,
                    Price = product.Price
                });
            }

            await _context.SaveChangesAsync(); // Save changes to the database

            TempData["SuccessMessage"] = "Product successfully added to the cart!"; 
            // Display a success message


            return RedirectToAction(nameof(Index)); // Redirect to the product listing
        }
    }
}
