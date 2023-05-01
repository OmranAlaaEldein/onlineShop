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
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Shopping.Data;
using AutoMapper;
using Shopping.Models;
using Shopping.Services.Admin.BradeSer;
using Shopping.Data.Reposotries;
using Shopping.Services.Admin.CategorySer;
using Shopping.Services.Admin.ProductSer;
using Shopping.Services.Admin.ItemSer;
using Shopping.Extensions;
using Shopping.Services.Logging;
using NLog;
using System.IO;
using Shopping.Services.Customer;
using System.Globalization;

namespace Shopping
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //string mySqlConnectionStr = Configuration.GetConnectionString("mySqlConnectionStr");
            //IServiceCollection serviceCollection = services.AddDbContextPool<ApplicationDbContext>(options => options.UseMySql(mySqlConnectionStr));

            //Entity FrameWork
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            
            //identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            var key = Encoding.ASCII.GetBytes(Configuration["JwtConfig:Secret"]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = false,
                ClockSkew = TimeSpan.Zero
            };

            services.AddSingleton(tokenValidationParameters);
            // Adding Authentication  
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(jwt => {
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = tokenValidationParameters;
                });

            
            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Shoop", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IRBrade, RBrade>();
            services.AddTransient<IBradeServices, BradeServices>();

            services.AddScoped<IRCategory, RCategory>();
            services.AddTransient<ICategoryServices, CategoryServices>();

            services.AddScoped<IRProduct, RProduct>();
            services.AddTransient<IProductServices, ProductServices>();

            services.AddScoped<IRItem, RItem>();
            services.AddTransient<IItemServices, ItemServices>();

            services.AddScoped<IROrder,ROrder>();
            services.AddTransient<IOrderServices, OrderServices>();


            services.AddCors(c => {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddControllers();
            services.AddMemoryCache();

            services.AddLocalization(x => { x.ResourcesPath = "Resources"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopOnline v1"));
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            
            //app.UseCookiePolicy(new CookiePolicyOptions() { 
            //OnDeleteCookie=DeleteBehavior.
            //});
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseCustomMiddleware();

            var logger = app.ApplicationServices.GetRequiredService<ILoggerManager>();
            app.ConfigureExceptionHandler(logger);

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var supportedCultures = new[] { "en-US", "ar" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            //localizationOptions.ApplyCurrentCultureToResponseHeaders
            app.UseRequestLocalization(localizationOptions);
            


        }

     
    }
}
