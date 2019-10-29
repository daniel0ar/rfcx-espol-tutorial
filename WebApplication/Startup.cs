using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NonFactors.Mvc.Grid;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.DbModels;
using WebApplication.IRepository;
using WebApplication.Repository;
using Serilog;
using Microsoft.Extensions.Logging;
using WebApplication.Helpers;
using WebApplication.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = (IConfigurationRoot)configuration;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Core.MakeFilesFolder();
            Core.MakeSpeciesFolder();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Core.FilesFolderPath));
            services.AddMvc();
            services.AddSession();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(RolePolicy.PoliticaRoleAdministrador,
                    policy => policy.RequireRole(Role.Admin));

                options.AddPolicy(RolePolicy.PoliticaRoleInvitado,
                    policy => policy.RequireRole(Role.Invitado));

                options.AddPolicy(RolePolicy.PoliticaRoleTodos,
                    policy => policy.RequireRole(Role.Invitado, Role.Admin));

                options.AddPolicy(RolePolicy.PoliticaRoleAdminDev,
                    policy => policy.RequireRole(Role.Admin, Role.Desarrollador));
            });

            // configure DI for application services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();

            IFileProvider physicalProvider = new PhysicalFileProvider(Core.getServerDirectory());
            services.AddSingleton<IFileProvider>(physicalProvider);
            services.Configure<Settings>(
            options =>
                {
                    options.iConfigurationRoot = Configuration;
                });
            services.AddTransient<IIncidentRepository, IncidentRepository>();
            services.AddTransient<IAlertRepository, AlertRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<IAudioRepository, AudioRepository>();
            services.AddTransient<IStationRepository, StationRepository>();
            services.AddTransient<ILabelRepository, LabelRepository>();
            services.AddTransient<ISensorRepository, SensorRepository>();
            services.AddTransient<IDataRepository, DataRepository>();
            services.AddTransient<IInfoSensoresRepository, InfoSensoresRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAnimalRepository, AnimalRepository>();

            services.AddSingleton<AuthService>();
            //services.AddSingleton<UserRepository>();

            services.AddCors(options =>
                {
                    options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                    });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseMiddleware<PreRequestModifications>();

            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }*/

            app.UseDeveloperExceptionPage();
            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=authentication}/{action=Index}");
            });
            app.UseCookiePolicy();
            app.UseStaticFiles();
            //app.UseMvc();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Core.getServerDirectory(), "resources")),
                RequestPath = "/resources"
            });

            // app.Map("/hello", HandleHello);
            // app.Map("/sendgz", GZReceiver.HandleGZFile);
            // app.Map("/getzip", GZReceiver.HandleSendZipFile);
            // app.Run(async (context) => {
            //     await context.Response.WriteAsync("Rfcx Server is running");
            // });

            loggerFactory.AddSerilog();

            //app.UseMiddleware<PreRequestModifications>();


            StaticFileOptions option = new StaticFileOptions();
            FileExtensionContentTypeProvider contentTypeProvider = (FileExtensionContentTypeProvider)option.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings.Add(".unityweb", "application/octet-stream");
            option.ContentTypeProvider = contentTypeProvider;
            app.UseStaticFiles(option);
        }
    }
}
