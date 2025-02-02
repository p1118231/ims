using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using inventory.Support;
using Microsoft.EntityFrameworkCore;
using inventory.Data;
using CsvHelper;
using System.Globalization;
using inventory.Models;
using inventory.Services.ProductRepo;
using Microsoft.AspNetCore.DataProtection;
using inventory.Services.CategoryRepo;
using inventory.Services.SupplierRepo;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the HTTP request pipeline.
builder.Services.ConfigureSameSiteNoneCookies();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();


//builder.Services.AddDataProtection()
  //  .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys")) // Use a shared directory for load-balanced systems
   // .SetApplicationName("MyApp"); // Optional: To ensure isolation between apps


//add database context
builder.Services.AddDbContext<ProductContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "product.db");
        options.UseSqlite($"Data Source={dbPath}");
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
    else
    {
         var cs = builder.Configuration.GetConnectionString("ProductContext");
        options.UseSqlServer(cs, sqlServerOptionsAction: sqlOptions =>
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(6),
                errorNumbersToAdd: null
            )
        );
    }
});

// Add session services
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true; // Necessary for GDPR compliance
        options.IdleTimeout = TimeSpan.FromMinutes(20); // Adjust timeout as needed
    });

 builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.SaveTokens = true; 

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("read:products");

    options.CallbackPath = new PathString("/callback");
    options.ClaimsIssuer = "Auth0";

    options.SaveTokens = true;

    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProviderForSignOut = context =>
        {
            var logoutUri = $"https://{builder.Configuration["Auth0:Domain"]}/v2/logout?client_id={builder.Configuration["Auth0:ClientId"]}";
            context.Response.Redirect(logoutUri);
            context.HandleResponse();

            return Task.CompletedTask;
        }
    };
});  

var app = builder.Build();

if(builder.Environment.IsDevelopment()){

// Use this code to insert products into the database once
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();

    // Check if the database is empty, then insert products from CSV
    if (!dbContext.Product.Any())
    {
        
        var csvFilePath = "wwwroot/csvFiles/products_data.csv";  // Adjust this path if necessary
        using (var reader = new StreamReader(csvFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var products = csv.GetRecords<Product>().ToList();

            // Insert products into the database
            
           
        }

        var categoryCsvFilePath = "wwwroot/csvFiles/category_data.csv";  // Adjust this path if necessary
        using (var reader = new StreamReader(categoryCsvFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var categories = csv.GetRecords<Category>().ToList();

            // Insert products into the database
            dbContext.Categories.AddRange(categories);
            
        }

        var supplierCsvFilePath = "wwwroot/csvFiles/supplier_data.csv";  // Adjust this path if necessary
        using (var reader = new StreamReader(supplierCsvFilePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var suppliers = csv.GetRecords<Supplier>().ToList();

            // Insert products into the database
            dbContext.Suppliers.AddRange(suppliers);
            
        }

        dbContext.SaveChanges();
    }
}
}


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseBrowserLink(); // Add this line
   
}
else{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
