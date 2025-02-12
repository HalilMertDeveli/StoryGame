using FluentValidation;
using FluentValidation.AspNetCore;
using MurderGame.Business.DependencyResolvers.Microsoft;
using MurderGame.Business.Services;
using MurderGame.Business.ValidationRules;
using MurderGame.Entities.Domains;

namespace MurderGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //connection string lower
            builder.Services.AddDependencies(builder.Configuration);
            //fluent validation lower
            builder.Services.AddValidatorsFromAssemblyContaining<SingUpDtoValidator>();
            builder.Services.AddScoped<SignUpService>();
            builder.Services.AddTransient<IValidator<ApplicationUser>, SingUpDtoValidator>();





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

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
