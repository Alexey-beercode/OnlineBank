using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OnlineBank.DataManagment;
using OnlineBank.DataManagment.Repositories.Implementations;
using OnlineBank.Service.Service;
using OnlineBank.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<DepositTypeRepository>();
builder.Services.AddScoped<DepositRepository>();
builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<TransactionTypeRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<DepositService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<RoleService>();
;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
string? deviceConnection = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(deviceConnection); });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Authentication";
        options.LoginPath = "/User/Authorization";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();