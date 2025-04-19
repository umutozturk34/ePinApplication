using System.Linq;
using System.Collections.Generic;

namespace EpinHell.Models
{
    // Represents a shopping cart for a user
    public class Cart
    {
        // Unique identifier for the cart
        public int Id { get; set; }

        // The ID of the user associated with the cart
        public string UserId { get; set; }

        // Collection of items in the cart
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Computed property to calculate the total price of all items in the cart
        public decimal TotalPrice
        {
            get
            {
                return CartItems.Sum(item => item.TotalPrice); // Sum of TotalPrice for each cart item
            }
        }

        // Adds a product to the cart. If the product already exists, it increments the quantity.
        public void AddItem(Product product)
        {
            var existingItem = CartItems.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity++; // Increment quantity if the item already exists
            }
            else
            {
                // Add a new cart item if the product is not already in the cart
                CartItems.Add(new CartItem
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1, // Default quantity for new items
                    Price = product.Price, // Price of the product
                });
            }
        }

        // Removes an item from the cart based on its product ID
        public void RemoveItem(int productId)
        {
            var item = CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                CartItems.Remove(item); // Remove the item if it exists
            }
        }

        // Updates the quantity of a specific item in the cart
        public void UpdateItemQuantity(int productId, int quantity)
        {
            var item = CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity; // Update quantity if valid
            }
            else
            {
                RemoveItem(productId); // Remove the item if quantity is invalid (e.g., 0)
            }
        }
    }
}
