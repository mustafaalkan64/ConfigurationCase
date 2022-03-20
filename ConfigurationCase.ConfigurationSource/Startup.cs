using AutoMapper;
using Configuration.Core.Mapper;
using ConfigurationCase.ConfigurationSource.Abstracts;
using ConfigurationCase.ConfigurationSource.Consumers;
using ConfigurationCase.ConfigurationSource.Services;
using ConfigurationCase.Core.Caching;
using ConfigurationCase.Core.Models;
using ConfigurationCase.DAL;
using Hangfire;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationCase.ConfigurationSource
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ConfigurationCase.ConfigurationSource", Version = "v1" });
            });

            services.AddTransient<IConfigurationJobService, ConfigurationJobService>();
            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire")));
            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<ConfigurationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var redisConfig = Configuration.GetSection("RedisConfig");
            services.Configure<RedisServerConfig>(redisConfig);

            services.AddTransient<IRedisCacheService, RedisCacheService>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<ReadConfigurationsConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetConnectionString("RabbitMQ"));

                    cfg.ReceiveEndpoint("ReadConfigurationsRequestedQeueu", e =>
                    {
                        e.ConfigureConsumer<ReadConfigurationsConsumer>(context);
                    });
                });
            });

            services.AddMassTransitHostedService();

            //Automapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MapProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConfigurationCase.ConfigurationSource v1"));
            }
            app.UseHangfireDashboard();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });
        }
    }
}
