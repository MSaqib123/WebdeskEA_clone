using WebdeskEA.DataAccess.DbInitilizer;
using WebdeskEA.DataAccess;
using WebdeskEA.Domain.ServiceMiddlwares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using WebdeskEA.Core.Middleware;
using Rotativa.AspNetCore;

namespace WebdeskEA.Core.Middlware
{
    public static class MiddlewareConfigurationExtensions
    {
        public static IApplicationBuilder ConfigureMiddleware(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();

            // Custom middleware to handle exceptions
            app.UseMiddleware<CustomExceptionHandlingMiddleware>();

            // Error handling
            app.UseStatusCodePagesWithReExecute("/Settings/Error/Error", "?statusCode={0}");

            if (!env.IsDevelopment())
            {
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            // Add SessionInitializationMiddleware here
            app.UseMiddleware<SessionInitializationMiddleware>();

            // Corrected environment variable usage for Rotativa setup
            RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");
            return app;
        }

        public static IApplicationBuilder ConfigureEndpoints(this IApplicationBuilder app)
        {
            // Seed DB
            SeedDatabase(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                // Handle favicon requests
                endpoints.MapGet("/favicon.ico", async context =>
                {
                    context.Response.StatusCode = 204; // No Content
                    return;
                });
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapGet("/", async context =>
                {
                    if (context.Request.Path == "/")
                    {
                        if (context.User!.Identity!.IsAuthenticated)
                        {
                            //var companyId = context.User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
                            //var tenantId = context.User.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
                            //Console.WriteLine($"Authenticated user with role:, CompanyId: {companyId}, TenantId: {tenantId}");

                            if (context.User.IsInRole("SuperAdmin"))
                            {
                                context.Response.Redirect("/Dashboard/DSB_SuperAdmin/Index");
                            }
                            else
                            {
                                context.Response.Redirect("/Dashboard/DSB_Home/Index"); // Default user dashboard
                            }
                        }
                        else
                        {
                            context.Response.Redirect("/Identity/Account/Login?ReturnUrl=%2F");
                        }
                    }

                    await Task.CompletedTask; // Ensure proper task handling
                });
            });

            return app;
        }


        private static void SeedDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetRequiredService<WebdeskEADBContext>();
                    context.Database.Migrate();

                    var dbInitilizer = scope.ServiceProvider.GetRequiredService<IDbInitilizer>();
                    dbInitilizer.Initilize();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                    throw;
                }
            }
        }

    }
}
