using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Recollectable.Core.DTOs.Collectables;
using Recollectable.Core.DTOs.Collections;
using Recollectable.Core.DTOs.Locations;
using Recollectable.Core.DTOs.Users;
using Recollectable.Core.Entities.Collectables;
using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Entities.Locations;
using Recollectable.Core.Entities.ResourceParameters;
using Recollectable.Core.Entities.Users;
using Recollectable.Core.Interfaces.Repositories;
using Recollectable.Core.Interfaces.Services;
using Recollectable.Core.Services.Common;
using Recollectable.Core.Shared.Interfaces;
using Recollectable.Core.Shared.Services;
using Recollectable.Infrastructure.Data;
using Recollectable.Infrastructure.Data.Repositories;
using System.Collections.Generic;
using System.Linq;

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

                var jsonOutputFormatter = options.OutputFormatters
                    .OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/json+hateoas");
                }
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register DbContext
            services.AddDbContext<RecollectableContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("RecollectableConnection")));

            // Register repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICollectableRepository, CollectableRepository>();
            services.AddScoped<IRepository<User, UsersResourceParameters>, UserRepository>();
            services.AddScoped<IRepository<Collection, CollectionsResourceParameters>, CollectionRepository>();
            services.AddScoped<IRepository<Coin, CurrenciesResourceParameters>, CoinRepository>();
            services.AddScoped<IRepository<Banknote, CurrenciesResourceParameters>, BanknoteRepository>();
            services.AddScoped<IRepository<Condition, ConditionsResourceParameters>, ConditionRepository>();
            services.AddScoped<IRepository<Country, CountriesResourceParameters>, CountryRepository>();
            services.AddScoped<IRepository<CollectorValue, CollectorValuesResourceParameters>, CollectorValueRepository>();

            // Register Helper Classes
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory
                    .GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
            services.AddTransient<IControllerService, ControllerService>();

            // Register HTTP Caching
            services.AddHttpCacheHeaders(
                (expirationModelOptions) => 
                {
                    expirationModelOptions.MaxAge = 1;
                },
                (validationModelOptions) =>
                {
                    validationModelOptions.MustRevalidate = true;
                });
            services.AddResponseCaching();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 1000000000,
                        Period = "1s"
                    }
                };
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
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
                cfg.CreateMap<User, UserDto>().ForMember(dest => dest.Name, 
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
                cfg.CreateMap<UserCreationDto, User>();
                cfg.CreateMap<UserUpdateDto, User>();
                cfg.CreateMap<User, UserUpdateDto>();
                cfg.CreateMap<Collection, CollectionDto>();
                cfg.CreateMap<CollectionCreationDto, Collection>();
                cfg.CreateMap<CollectionUpdateDto, Collection>();
                cfg.CreateMap<Collection, CollectionUpdateDto>();
                cfg.CreateMap<CollectionCollectable, CollectableDto>();
                cfg.CreateMap<CollectableCreationDto, CollectionCollectable>();
                cfg.CreateMap<CollectableUpdateDto, CollectionCollectable>();
                cfg.CreateMap<CollectionCollectable, CollectableUpdateDto>();
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
            app.UseIpRateLimiting();
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseMvc();
        }
    }
}