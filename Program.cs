using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using WeatherApp.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Legger til dette for å bruke ApplicationDbContext med SQLite
builder.Services.AddRazorPages();

// Legger til dette for å bruke ApplicationDbContext med SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
//------------------------------------------------------------------
//                                    (API)
// Denne er for å bruke LocationService som en singleton service
builder.Services.AddHttpClient<LocationService>();
// 
builder.Services.AddHttpClient<WeatherForecastService>();
//------------------------------------------------------------------
//    Legger til dette for å bruke ApplicationDbContext med SQLite
builder.Services.AddDefaultIdentity<Microsoft.AspNetCore.Identity.IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

//------------------------------------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
//...................................................
//Legger til autentisering og autorisasjon i applikasjonen. Dette er nødvendig for å håndtere brukerinnlogging og tilgangskontroll.
app.UseAuthentication();
app.UseAuthorization();
//...................................................

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
// Legger til rute for Razor Pages, som brukes for å håndtere brukerautentisering og autorisasjon i applikasjonen.    
app.MapRazorPages();

app.Run();
