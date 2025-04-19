using System.ComponentModel.DataAnnotations;

namespace EpinHell.Models
{
    // ViewModel for user registration, containing user input fields and validation rules
    public class RegisterViewModel
    {
        // User's email address with validation for required, format, and pattern
        [Required(ErrorMessage = "Email is required.")] // Ensures the email field is not left empty
        [EmailAddress(ErrorMessage = "Invalid email address.")] // Validates the input is a valid email format
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Please write a real Email Address!")]
        public string Email { get; set; }

        // User's chosen username with validation for required input
        [Required(ErrorMessage = "Username is required.")] // Ensures the username field is not left empty
        public string Username { get; set; }

        // User's chosen password with strong password validation rules
        [Required(ErrorMessage = "Password is required.")] // Ensures the password field is not left empty
        [DataType(DataType.Password)] // Specifies the data type as a password, hiding input characters
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{6,}$",
            ErrorMessage = "Your password must be at least 6 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.")] // Enforces a strong password policy
        public string Password { get; set; }

        // Confirmation password with validation to match the original password
        [Required(ErrorMessage = "Confirmation password is required.")] // Ensures the confirm password field is not left empty
        [DataType(DataType.Password)] // Specifies the data type as a password, hiding input characters
        [Compare("Password", ErrorMessage = "Passwords do not match.")] // Ensures the confirm password matches the password field
        public string ConfirmPassword { get; set; }
    }
}
