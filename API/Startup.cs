using System.Text;
using Application;
using Application.Users;
using AutoMapper;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
            
            var builder = services.AddIdentityCore<AppUser>();
            builder = new IdentityBuilder(builder.UserType, typeof(AppRole), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddSignInManager<SignInManager<AppUser>>();
            
            // var key = new SymmetricSecurityKey(Encoding.UTF8.
            //     GetBytes(_configuration["AppSettings:Token"]));
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes("super secret key"));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddMvc(opt => 
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
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
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
