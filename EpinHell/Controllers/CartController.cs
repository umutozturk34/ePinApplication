using EpinHell.Data;
using EpinHell.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EpinHell.Controllers
{
    [Authorize] // Ensure only authenticated users can access this controller
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Constructor to inject database context and user manager
        public CartController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Action to handle the purchase process
        public async Task<IActionResult> Purchase()
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to login if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the user's cart including its items
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                // Remove all items from the cart
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync(); // Save changes to the database
            }

            // Redirect to the thank-you page
            return View("Purchase"); // Assumes a Purchase.cshtml view exists
        }

        // Action to display the user's cart
        public async Task<IActionResult> ViewCart()
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to login if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the user's cart with its items and their associated products
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .ThenInclude(ci => ci.Product)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // Return an empty cart view if the cart does not exist
                return View(new Cart());
            }

            // Return the cart view populated with the user's cart data
            return View(cart);
        }

        // Action to update the quantity of a cart item
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, string quantity)
        {
            // Find the cart item by its ID
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                // Return a 404 response if the cart item is not found
                return NotFound();
            }

            // Adjust the quantity based on the input parameter
            if (quantity == "increase")
            {
                cartItem.Quantity += 1; // Increase the quantity
            }
            else if (quantity == "decrease" && cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1; // Decrease the quantity (minimum is 1)
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect back to the cart view
            return RedirectToAction("ViewCart");
        }

        // Action to remove an item from the cart
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            // Find the cart item by its ID
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                // Return a 404 response if the cart item is not found
                return NotFound();
            }

            // Get the current user's ID
            var userId = _userManager.GetUserId(User);
            // Retrieve the user's cart with its items
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .FirstOrDefaultAsync(c => c.UserId == userId);

            // Ensure the cart exists and contains the item to be removed
            if (cart == null || !cart.CartItems.Contains(cartItem))
            {
                return NotFound();
            }

            // Remove the item from the cart
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync(); // Save changes to the database

            // Redirect back to the cart view
            return RedirectToAction("ViewCart");
        }
    }
}
