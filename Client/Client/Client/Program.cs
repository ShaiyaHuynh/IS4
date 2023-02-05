using Client.Services;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            //configuration.GetValue<int>("Formatting:Number:Precision")

            // Add services to the container.
            builder.Services.AddControllersWithViews();
             builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            })
             .AddCookie("cookie")
             .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = configuration.GetValue<string>("InteractiveServiceSettings:AuthorityUrl");
                    options.ClientId = configuration.GetValue<string>("InteractiveServiceSettings:ClientId");
                    options.ClientSecret = configuration.GetValue<string>("InteractiveServiceSettings:ClientSecret");

                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.ResponseMode = "query";

                    // options.CallbackPath = "/signin-oidc"; // default redirect URI

                    // options.Scope.Add("oidc"); // default scope
                    // options.Scope.Add("profile"); // default scope
                    options.Scope.Add(configuration.GetValue<string>("InteractiveServiceSettings:Scopes:0"));
                    options.SaveTokens = true;
                }); 

            builder.Services.Configure<IdentityServerSettings>(builder.Configuration.GetSection("IdentityServerSettings"));
            builder.Services.AddSingleton<ITokenService, TokenService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            { app.UseDeveloperExceptionPage(); 
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //      name: "default",
            //      pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.Run();
        }
    }
}