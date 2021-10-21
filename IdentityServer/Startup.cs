using IdentityServer.Context;
using IdentityServer.CustomDescriber;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                //.AddErrorDescriber<CustomErrorDescriber>() -> Bunu addEntityFrameworkStores'den önce yazmalýyýz daha sonra bizim türkçe mesajlarýmýz kullanýlacak. & Ve password optionslarýný commentlemeliyiz customErrorDesriber'i eklerken
                opt.Password.RequireDigit = false; //mutlaka sayý içermesi için
                opt.Password.RequiredLength = 1;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<IdentityContext>();
            services.ConfigureApplicationCookie(opt =>
            {
                opt.Cookie.HttpOnly = false;
                opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict; //Sadece ilgili domain ile kullanýlabilir
                opt.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest; //sadece http'de çalýþýr always dersek | sameasrequest der isek gönderilen isteðe göre karþýlýk olur
                opt.Cookie.Name = "Cookie";
                opt.ExpireTimeSpan = TimeSpan.FromDays(25); //25 gün boyunca bilgiyi hatýrlar
                opt.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Home/SignIn");
            });
            services.AddDbContext<IdentityContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("IdentityServerConStr"));
            });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/node_mdoules",
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "node_modules"))
            });

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
