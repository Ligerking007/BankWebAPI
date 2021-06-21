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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
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
