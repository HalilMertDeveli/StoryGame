using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MurderGame.Business.DependencyResolvers.Microsoft;
using MurderGame.Business.Services.Email;
using MurderGame.Business.Services.Facebook;
using MurderGame.Business.Services.Github;
using MurderGame.Business.Services.Google;
using MurderGame.Business.Services.Twitter;
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
            builder.Services.AddScoped<TwitterSignUpService>();
            builder.Services.AddScoped<GitHubSignInService>();
            builder.Services.AddScoped<GitHubSignUpService>();

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Database Context and Identity Configuration
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptions =>
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null)
                ));


            builder.Services.AddIdentity<ApplicationUser, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Dependency Injection
            builder.Services.AddDependencies(builder.Configuration);

            // FluentValidation Configuration
            builder.Services.AddValidatorsFromAssemblyContaining<SingUpDtoValidator>();
            builder.Services.AddScoped<SignUpService>();

            //google things
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
    .AddCookie() // Cookie authentication is required for external logins
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "default-client-id";
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "default-client-secret";
        googleOptions.CallbackPath = "/signin-google";
    })
    .AddTwitter(twitterOptions =>
    {
        twitterOptions.ConsumerKey = builder.Configuration["Authentication:Twitter:ConsumerKey"] ?? "default-consumer-key";
        twitterOptions.ConsumerSecret = builder.Configuration["Authentication:Twitter:ConsumerSecret"] ?? "default-consumer-secret";
        twitterOptions.RetrieveUserDetails = true;
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        facebookOptions.CallbackPath = "/signin-facebook";
    })
    .AddOAuth("GitHub", githubOptions =>
    {
        githubOptions.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        githubOptions.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
        githubOptions.CallbackPath = new PathString("/signin-github");

        githubOptions.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        githubOptions.TokenEndpoint = "https://github.com/login/oauth/access_token";
        githubOptions.UserInformationEndpoint = "https://api.github.com/user";

        githubOptions.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        githubOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        githubOptions.ClaimActions.MapJsonKey("avatar_url", "avatar_url");

        githubOptions.Scope.Add("user:email");  // 🔹 Eklendi: GitHub'dan email alabilmek için kapsam eklendi.

        githubOptions.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await context.Backchannel.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(user.RootElement);


                var email = user.RootElement.TryGetProperty("email", out var emailProperty) ? emailProperty.GetString() : null;

                if (string.IsNullOrEmpty(email))  // 🔹 Eklendi: Email boşsa ek istek yap.
                {
                    var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
                    emailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
                    emailRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var emailResponse = await context.Backchannel.SendAsync(emailRequest);
                    emailResponse.EnsureSuccessStatusCode();

                    var emails = JsonDocument.Parse(await emailResponse.Content.ReadAsStringAsync());
                    //email burada
                    var primaryEmail = emails.RootElement.EnumerateArray().FirstOrDefault(e => e.GetProperty("primary").GetBoolean()).GetProperty("email").GetString();

                    if (!string.IsNullOrEmpty(primaryEmail))
                    {
                        context.Identity.AddClaim(new Claim(ClaimTypes.Email, primaryEmail));  // 🔹 Email bilgisi başarıyla alındı.
                    }
                    else
                    {
                        throw new Exception("Email bilgisi alınamadı.");  // 🔹 Hata fırlatıldı.
                    }
                }
            }
        };



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
