using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Application.Users;
using AutoMapper;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Errors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });
            services.AddDataAccessServices(_configuration.GetConnectionString("DefaultConnection"));
            services.AddApplicationServices();
            services.AddInfrastructureServices();
            services.AddAutoMapper();
            services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<DataContext>();
            services.AddMvc(opt => 
            {
                opt.Filters.Add(typeof(ValidatorActionFilter));
            })
            .AddFluentValidation(cfg => 
            {
                cfg.RegisterValidatorsFromAssemblyContaining(typeof(Login));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
