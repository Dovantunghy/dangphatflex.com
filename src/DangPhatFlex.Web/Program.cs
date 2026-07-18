using System.Text.Encodings.Web;
using System.Text.Unicode;
using DangPhatFlex.Web.Data;
using DangPhatFlex.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISlugService, SlugService>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// Responses are always served as UTF-8, so allow the full Unicode range through Razor's
// HtmlEncoder instead of the ASCII-only default, which HTML-entity-encodes every
// Vietnamese diacritic in dynamic (@expression) output.
builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Public" });

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var slugService = scope.ServiceProvider.GetRequiredService<ISlugService>();
    context.Database.Migrate();
    DbSeeder.Seed(context, slugService);
}

app.Run();

public partial class Program { }
