using EpinHell.Data; // Import the data context for database operations
using Microsoft.AspNetCore.Identity; // Import ASP.NET Identity for authentication and user management
using Microsoft.EntityFrameworkCore; // Import Entity Framework Core for database context

var builder = WebApplication.CreateBuilder(args); // Create a builder for configuring the application

// Configure the application's database context to use SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); // Connect to the SQLite database defined in the app settings

// Add ASP.NET Identity services for user and role management
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>() // Use the AppDbContext for storing user data
    .AddDefaultTokenProviders(); // Add token providers for things like password reset and email confirmation

// Configure Identity options for password complexity
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password requirements: must contain a digit, special character, and both upper and lowercase letters
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6; // Minimum length of 6 characters
    options.Password.RequireNonAlphanumeric = true; // Require at least one special character
    options.Password.RequireUppercase = true; // Require at least one uppercase letter
    options.Password.RequireLowercase = true; // Require at least one lowercase letter
});

// Add MVC services for controllers and views
builder.Services.AddControllersWithViews();

var app = builder.Build(); // Build the application

// If the environment is not Development, use a global exception handler and enable HSTS (HTTP Strict Transport Security)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Redirect to the error page in case of an exception
    app.UseHsts(); // Enforce HTTP Strict Transport Security (HSTS) for better security
}

// Enable HTTPS redirection to ensure all traffic is over HTTPS
app.UseHttpsRedirection();

// Serve static files (e.g., images, CSS, JS)
app.UseStaticFiles();

// Set up routing for the application
app.UseRouting();

// Enable authentication and authorization middleware
app.UseAuthentication(); // Enables authentication to validate user identity
app.UseAuthorization(); // Enables authorization to control access to resources based on user roles and permissions

// Map default controller route
app.MapDefaultControllerRoute();

// Run the application
app.Run();
