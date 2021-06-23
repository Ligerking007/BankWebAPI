using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository.Models.BankDB;
using Repository.Models.MappingProfile;
using Core.Interfaces;
using Core.Services;
using System.Reflection;
using Microsoft.OpenApi.Models;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Net.Http.Headers;

namespace BankWebAPI
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

            services.AddMemoryCache();
            RepositoryDependencySolver.Init(services);
            ServiceDependencySolver.Init(services);

            services.AddDbContext<BankDBContext>(options =>
              options.UseSqlServer(
                  this.Configuration.GetConnectionString("DefaultConnection")));

            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DomainProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(typeof(Startup));

            services.AddControllers();

            services.AddSwaggerGen(swagger =>
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank Web API", Version = "v1" });
                    var basePath = AppContext.BaseDirectory;
                    var xmlPath = Path.Combine(basePath, "BankWebApi.xml");
                    c.IncludeXmlComments(xmlPath);
                });

                //swagger.SwaggerDoc("BankWebAPI", new OpenApiInfo { Title = "Bank Web API", Version = "v1" });
                //swagger.AddSecurityDefinition("Bearer",
                //      new OpenApiSecurityScheme
                //      {
                //          In = "header",
                //          Name = "Authorization",
                //          Type = "apiKey"
                //      });
                //swagger.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                //    { "Bearer", Enumerable.Empty<string>() },
                //});

                //Set the comments path for the Swagger JSON and UI.
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //swagger.IncludeXmlComments(xmlPath);
            });
            services.AddMvc();
            services.AddControllersWithViews();
            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseHttpsRedirection();
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        const int cacheDurationInSeconds = 60 * 60 * 24 * 365; // 1 year
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                            $"public,max-age={cacheDurationInSeconds}";
                    }
                });
            app.UseRouting();

            app.UseAuthorization();
           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                    );
            });

            if (true)
            {
                app.UseDeveloperExceptionPage();
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                //app.UseSwagger();
                //app.UseSwaggerUI(c =>
                //{
                //    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                //    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Bank Web API");
                //    c.DefaultModelsExpandDepth(-1);
                //});

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank Web API");
                });
            }
        }
    }
    public static class ServiceDependencySolver
    {
        public static void Init(IServiceCollection services)
        {
            var iservice = Assembly.GetAssembly(typeof(Core.Interfaces.ICustomerAccountService));
            var service = Assembly.GetAssembly(typeof(Core.Services.CustomerAccountService));
            foreach (Type mytype in iservice.GetTypes().Where(mytype => mytype.IsInterface))
            {
                foreach (Type myImple in service.GetTypes().Where(myImple => mytype.IsAssignableFrom(myImple)))
                {
                    services.AddScoped(mytype, myImple);
                }
            }
            services.AddLogging();
            //services.AddSingleton() ;
        }
    }
    public static class RepositoryDependencySolver
    {
        public static void Init(IServiceCollection services)
        {


            // MCGasoline
            var iservice = Assembly.GetAssembly(typeof(Repository.Interfaces.BankDB.ICustomerAccountRepository));
            var service = Assembly.GetAssembly(typeof(Repository.Entities.BankDB.CustomerAccountRepository));
            foreach (Type mytype in iservice.GetTypes().Where(mytype => mytype.IsInterface))
            {
                foreach (Type myImple in service.GetTypes().Where(myImple => mytype.IsAssignableFrom(myImple)))
                {
                    services.AddScoped(mytype, myImple);
                }
            }
        }
    }
}
