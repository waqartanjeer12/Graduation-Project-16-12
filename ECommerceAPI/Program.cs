using ECommerceCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using ECommerceInfrastructure.Configurations.Data;
using ECommerceInfrastructure.Configurations.Identity;
using ECommerceCore.Interfaces;
using ECommerceInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.FileProviders;



var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Set up logging with Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"))
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));  // تجاهل التحذير



//there is interfaces and implemintations must addedd in the services to can use methods like createAsync i use the above line
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;

    //password setting
    // Allow Arabic letters and spaces

}).AddEntityFrameworkStores<AppIdentityDbContext>()
//.AddEntityFrameworkStores<AppIdentityDbContext>(); it contain interface and implmentation
.AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()    // This allows any origin
              .AllowAnyHeader()   // Allow any headers
              .AllowAnyMethod();  // Allow any HTTP method
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

// Use Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/*IServiceCollection serviceCollection = builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});*/

app.UseHttpsRedirection();

// Apply CORS policy before authentication and authorization
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();


var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "files", "images");

// Ensure the folder exists
if (!Directory.Exists(folderPath))
{
    Directory.CreateDirectory(folderPath);
}

// Log the folder path for debugging
Console.WriteLine($"Image folder path: {folderPath}");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "files")  // Pointing to the "files" directory, not "images"
    ),
    RequestPath = "/files"  // This will make images accessible via /files/{fileName}
});






app.MapControllers();

app.Run();