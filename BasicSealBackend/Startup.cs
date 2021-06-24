using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BasicSealBackend.Data;
using BasicSealBackend.Data.Interface;
using BasicSealBackend.Helpers;
using BasicSealBackend.Models;
using BasicSealBackend.Services;
using BasicSealBackend.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BasicSealBackend
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
            services.AddControllers();

            services.AddDbContextPool<DataContext>(options => options
             .UseMySQL(Configuration.GetConnectionString("DefaultMysqlConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;

                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                       .GetBytes(Configuration.GetSection("AppSettings:JWT:Secret").Value)),
                       ValidateIssuer = true,
                       ValidateAudience = false,
                       ValidIssuer = Configuration["AppSettings:JWT:ValidIssuer"]
                   };
               });
            /*
            services.AddAuthorization(options =>
            {
                options.AddPolicy("BasicApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("BasicScope", "BasicSealManagement");
                });
            });
            */
            services.AddCors();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddScoped<ISoftwareLicensesRepository, SoftwareLicensesRepository>();
            services.AddScoped<ISoftwaresRepository, SoftwaresRepository>();
            services.AddScoped<ISoftwareVersionsRepository, SoftwareVersionsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();

            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IEncryptionService, EncryptionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IManagementService, ManagementService>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.Migrate();
            }
            //app.UseHsts();
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
