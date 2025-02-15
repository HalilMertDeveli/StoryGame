using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MurderGame.Business.DependencyResolvers.Microsoft;
using MurderGame.Business.Services;
using MurderGame.Business.ValidationRules;
using MurderGame.DataAccess.Context;
using MurderGame.Entities.Domains;

namespace MurderGame
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<GoogleSingInUpService>();
            builder.Services.AddScoped<FacebookSignUpService>();

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Database Context and Identity Configuration
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Dependency Injection
            builder.Services.AddDependencies(builder.Configuration);

            // FluentValidation Configuration
            builder.Services.AddValidatorsFromAssemblyContaining<SingUpDtoValidator>();
            builder.Services.AddScoped<SignUpService>();

            //google things
            builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "sad"; // Google API ClientId
                    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "sad"; // Google API ClientSecret
                    googleOptions.CallbackPath = "/signin-google"; // Bu path'e Google callback yapar
                    //yaprık
                });

            //facebook login
            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = builder.Configuration["Authentication:Facebook:ClientId"];
                    facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
                    facebookOptions.CallbackPath = "/signin-facebook";  // Bu path'e Facebook callback yapacak
                });
            


            var app = builder.Build();

            // Create Roles (Admin, Member)
            await CreateRoles(app);

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();  // Authentication Middleware
            app.UseAuthorization();   // Authorization Middleware

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static async Task CreateRoles(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                string[] roles = { "Admin", "Member" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new Role { Name = role });
                    }
                }
            }
        }
    }
}
