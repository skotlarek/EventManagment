using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using vectio.eventmanagement.api.db;
using vectio.eventmanagement.api.db.entities;
using vectio.eventmanagement.api.helpers;

namespace vectio.eventmanagement.api
{
    public class Startup
    {
        private readonly string vectio = "VectioCORSPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EventManagementDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("EventManagementDB"));
            });

            services.AddTransient<EmailHelper, EmailHelper>();


            services.AddCors(options =>
            {
                options.AddPolicy(vectio,
                builder =>
                {
                    builder
#if DEBUG
                    .AllowAnyOrigin()
#else
                    .WithOrigins("https://alumni.vectio.pl", "https://www.alumni.vectio.pl", "http://localhost:3000")
                    .AllowCredentials()
#endif
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    ;
                });
            });

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            })
               .AddDefaultTokenProviders()
               .AddEntityFrameworkStores<EventManagementDBContext>();


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.RequireHttpsMetadata = false;
               options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidAudience = "https://alumni.vectio.pl",
                   ValidIssuer = "https://alumni.vectio.pl",
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("81251570-7113-4D0A-AD50-6A3499643432")),
                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.Zero
               };
           });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pl-PL");
                //By default the below will be set to whatever the server culture is. 
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("pl-PL") };

                options.RequestCultureProviders = new List<IRequestCultureProvider>();
            });

            services.AddControllers();
            services.AddResponseCompression();

            //var provider = services.BuildServiceProvider();
            //var ctx = provider.GetRequiredService<EventManagementDBContext>();
            //var userManager = provider.GetRequiredService<UserManager<User>>();
            //var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            //foreach (var r in new[] { "administrator", "user" })
            //    if (!Task.Run(() => roleManager.RoleExistsAsync(r)).Result)
            //    {
            //        Task.Run(() => roleManager.CreateAsync(new IdentityRole(r))).Wait();
            //    }

            //var admin = new User
            //{
            //    Email = "admin@vectio.pl",
            //    SecurityStamp = Guid.NewGuid().ToString(),
            //    UserName = "admin@vectio.pl",
            //    Firstname = "John",
            //    Lastname = "Doe"
            //};
            //var found = Task.Run(() => userManager.FindByNameAsync(admin.UserName)).Result;
            //if (found == null)
            //{
            //    Task.Run(() => userManager.CreateAsync(admin, "Qaz123!@")).Wait();
            //    found = Task.Run(() => userManager.FindByNameAsync(admin.UserName)).Result;
            //    Task.Run(() => userManager.AddToRoleAsync(found, "administrator")).Wait();
            //}


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(vectio);
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseResponseCompression();
        }
    }
}
