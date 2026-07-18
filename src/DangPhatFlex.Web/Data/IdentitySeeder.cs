using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace DangPhatFlex.Web.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        const string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        var email = configuration["AdminSeed:Email"];
        var password = configuration["AdminSeed:Password"];
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return;

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
            return;

        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, adminRole);
    }
}
