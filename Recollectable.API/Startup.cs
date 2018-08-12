using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Recollectable.API.Models;
using Recollectable.Data;
using Recollectable.Data.Repositories;
using Recollectable.Domain;

namespace Recollectable.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => {
                options.ReturnHttpNotAcceptable = true;
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register DbContext
            services.AddDbContext<RecollectableContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("RecollectableConnection")));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICollectionRepository, CollectionRepository>();
            services.AddScoped<ICoinRepository, CoinRepository>();
            services.AddScoped<IBanknoteRepository, BanknoteRepository>();
            services.AddScoped<IConditionRepository, ConditionRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICollectorValueRepository, CollectorValueRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, RecollectableContext recollectableContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected server error occurred. " +
                            "Try again later.");
                    });
                });
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<UserCreationDto, User>();
                cfg.CreateMap<UserUpdateDto, User>();
                cfg.CreateMap<User, UserUpdateDto>();
                cfg.CreateMap<Collection, CollectionDto>();
                cfg.CreateMap<CollectionCreationDto, Collection>();
                cfg.CreateMap<CollectionUpdateDto, Collection>();
                cfg.CreateMap<Collection, CollectionUpdateDto>();
                cfg.CreateMap<Coin, CoinDto>();
                cfg.CreateMap<CoinCreationDto, Coin>();
                cfg.CreateMap<CoinUpdateDto, Coin>();
                cfg.CreateMap<Coin, CoinUpdateDto>();
                cfg.CreateMap<Banknote, BanknoteDto>();
                cfg.CreateMap<BanknoteCreationDto, Banknote>();
                cfg.CreateMap<BanknoteUpdateDto, Banknote>();
                cfg.CreateMap<Banknote, BanknoteUpdateDto>();
                cfg.CreateMap<Condition, ConditionDto>();
                cfg.CreateMap<ConditionCreationDto, Condition>();
                cfg.CreateMap<ConditionUpdateDto, Condition>();
                cfg.CreateMap<Condition, ConditionUpdateDto>();
                cfg.CreateMap<Country, CountryDto>();
                cfg.CreateMap<CountryCreationDto, Country>();
                cfg.CreateMap<CountryUpdateDto, Country>();
                cfg.CreateMap<Country, CountryUpdateDto>();
                cfg.CreateMap<CollectorValue, CollectorValueDto>();
                cfg.CreateMap<CollectorValueCreationDto, CollectorValue>();
                cfg.CreateMap<CollectorValueUpdateDto, CollectorValue>();
                cfg.CreateMap<CollectorValue, CollectorValueUpdateDto>();
            });

            recollectableContext.Database.Migrate();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}