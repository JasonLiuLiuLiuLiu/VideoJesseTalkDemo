using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using mvcCookieAuthSample.Data;
using Microsoft.EntityFrameworkCore;
using mvcCookieAuthSample.Models;
using mvcCookieAuthSample.Service;
using Microsoft.AspNetCore.Identity;

namespace mvcCookieAuthSample
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
            const string connectionString =
                "Server=127.0.0.1;Database=aspnet-IdentitySample-CE9DD12E-9C3B-4072-8E38-6F33420849CB;uid=root;pwd=123456";
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;


            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            //IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext

            services.AddIdentity<ApplicationUser, ApplicationUserRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddInMemoryApiResources(Config.GetResources())
                //.AddInMemoryClients(Config.GetClients())
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = builder =>
                            {
                                builder.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                            };
                    })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    {
                        builder.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                    };
                })
                .AddAspNetIdentity<ApplicationUser>()
                .Services.AddScoped<IProfileService, ProfileService>();




            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options => {
            //        options.LoginPath = "/Account/Login";
            //    });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 12;
            });

            services.AddScoped<ConsentService>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            InitData(app);

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }


        public void InitData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var configuration = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!configuration.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configuration.Clients.Add(client.ToEntity());
                    }
                }

                if (!configuration.ApiResources.Any())
                {
                    foreach (var api in Config.GetResources())
                    {
                        configuration.ApiResources.Add(api.ToEntity());
                    }
                }

                if (!configuration.IdentityResources.Any())
                {
                    foreach (var identity in Config.GetIdentityResources())
                    {
                        configuration.IdentityResources.Add(identity.ToEntity());
                    }
                }

                configuration.SaveChanges();
            }
        }
    }
}

