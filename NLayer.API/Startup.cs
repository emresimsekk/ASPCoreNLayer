using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Service;
using NLayer.Core.UnitOfWorks;
using NLayer.Data;
using NLayer.Data.Repositories;
using NLayer.Data.UnitOfWorks;
using NLayer.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using NLayer.API.Filters;
using Microsoft.AspNetCore.Diagnostics;
using NLayer.API.Dtos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NLayer.API
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
            
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<NotFoundFilter>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IService<>), typeof(Service.Services.Service<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
            { 
                options.UseSqlServer(Configuration
                    ["ConnectionStrings:SqlConStr"].ToString(), o => 
                    {
                        o.MigrationsAssembly("NLayer.Data");

                    });
            });
           

            services.AddControllers(o=> {

                o.Filters.Add(new ValidationFilter());
            }
                
                );
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NLayer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NLayer.API v1"));
             
            }

            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var error = context.Features.Get<IExceptionHandlerFeature>();

                    if (error != null)
                    {
                        var ex = error.Error;
                        ErrorDto errorDto = new ErrorDto();
                        errorDto.Status = 500;
                        errorDto.Errors.Add(ex.Message);
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorDto));
                    }

                });

            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
