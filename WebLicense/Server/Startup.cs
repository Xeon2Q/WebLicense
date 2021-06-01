using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Services;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Access;
using WebLicense.Core.Auxiliary;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic;
using WebLicense.Logic.Auxiliary;
using WebLicense.Server.Auxiliary.IdentityServices;
using WebLicense.Server.Auxiliary.Middlewares;
using WebLicense.Shared.Auxiliary.Claims;
using WebLicense.Shared.Auxiliary.Policies;
using WebLicense.Server.Auxiliary.Extensions;

namespace WebLicense.Server
{
    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureSettings(services);
            ConfigureDatabase(services);
            ConfigureLocalization(services);
            InjectServices(services);

            services.AddMediatR(LogicAssembly.Assembly);

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<User>(ConfigureIdentity)
                    .AddRoles<Role>()
                    .AddEntityFrameworkStores<DatabaseContext>()
                    .AddClaimsPrincipalFactory<ClaimsPrincipalFactory>();

            services.AddIdentityServer()
                    .AddApiAuthorization<User, DatabaseContext>(ConfigureApiAuthorization);
            
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove(JwtClaimTypes.Role);

            services.AddAuthentication()
                    .AddIdentityServerJwt()
                    .AddMicrosoftAccount(options =>
                    {
                        options.ClientId = config[$"{nameof(AuthenticationSettings)}:{nameof(AuthenticationSettings.Microsoft)}:{nameof(AuthenticationSettings.Microsoft.Id)}"];
                        options.ClientSecret = config[$"{nameof(AuthenticationSettings)}:{nameof(AuthenticationSettings.Microsoft)}:{nameof(AuthenticationSettings.Microsoft.Secret)}"];
                    });

            services.AddAuthorization(ConfigureAuthorization);

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<LogUserInformationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

        #region Methods

        private void ConfigureSettings(IServiceCollection services)
        {
            services.Configure<SmtpSettings>(config.GetSection(nameof(SmtpSettings)));
            
            services.Configure<IdentitySettings>(config.GetSection(nameof(IdentitySettings)));
            services.Configure<IdentitySettings.UserSettings>(config.GetSection($"{nameof(IdentitySettings)}:{nameof(IdentitySettings.User)}"));
            services.Configure<IdentitySettings.PasswordSettings>(config.GetSection($"{nameof(IdentitySettings)}:{nameof(IdentitySettings.Password)}"));
            services.Configure<IdentitySettings.SignInSettings>(config.GetSection($"{nameof(IdentitySettings)}:{nameof(IdentitySettings.SignIn)}"));
            services.Configure<IdentitySettings.LockoutSettings>(config.GetSection($"{nameof(IdentitySettings)}:{nameof(IdentitySettings.Lockout)}"));

            services.Configure<AuthenticationSettings>(config.GetSection(nameof(AuthenticationSettings)));
        }

        private void ConfigureDatabase(IServiceCollection services)
        {
            // MSSQL
            //const string connectionName = "GlobalConnection";
            //services.AddDbContext<DatabaseContext>(options => options.UseSqlServer($"name={connectionName}"));
            //services.AddDbContext<DbContext, DatabaseContext>(options => options.UseSqlServer($"name={connectionName}"));

            // SQLite
            services.AddDbContext<DatabaseContext>(options => options.UseSqlite("Filename=webLicense.db"));
            services.AddDbContext<DbContext, DatabaseContext>(options => options.UseSqlite("Filename=webLicense.db"));
        }

        private void ConfigureLocalization(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("en-GB");

                var cultures = new[] {new CultureInfo("de"), new CultureInfo("en")};

                options.ApplyCurrentCultureToResponseHeaders = true;
                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });
        }

        private void InjectServices(IServiceCollection services)
        {
            services.AddScoped<IMailManager, MailManager>();

            services.AddTransient<IProfileService, ClaimsProfileService>();
        }

        private void ConfigureIdentity(IdentityOptions options)
        {
            var settings = new IdentitySettings();
            config.GetSection(nameof(IdentitySettings)).Bind(settings);

            options.User.AllowedUserNameCharacters = settings.User.AllowedUserNameCharacters;
            options.User.RequireUniqueEmail = settings.User.RequireUniqueEmail;

            options.Password.RequireDigit = settings.Password.RequireDigit;
            options.Password.RequireLowercase= settings.Password.RequireLowercase;
            options.Password.RequireUppercase = settings.Password.RequireUppercase;
            options.Password.RequireNonAlphanumeric = settings.Password.RequireNonAlphanumeric;
            options.Password.RequiredUniqueChars = settings.Password.RequiredUniqueChars;
            options.Password.RequiredLength = settings.Password.RequiredLength;

            options.SignIn.RequireConfirmedAccount = settings.SignIn.RequireConfirmedAccount;
            options.SignIn.RequireConfirmedEmail = settings.SignIn.RequireConfirmedEmail;
            options.SignIn.RequireConfirmedPhoneNumber = settings.SignIn.RequireConfirmedPhoneNumber;

            options.Lockout.AllowedForNewUsers = settings.Lockout.AllowedForNewUsers;
            options.Lockout.MaxFailedAccessAttempts = settings.Lockout.MaxFailedAccessAttempts;
            options.Lockout.DefaultLockoutTimeSpan = settings.Lockout.DefaultLockoutTimeSpan;
        }

        private void ConfigureApiAuthorization(ApiAuthorizationOptions options)
        {
            void AddWLClaims(ICollection<string> claims)
            {
                claims.Add(WLClaims.OwnAccount.CanLoginExternal.ClaimType);
                claims.Add(WLClaims.OwnAccount.CanResetPassword.ClaimType);
                claims.Add(WLClaims.OwnAccount.CanChangePassword.ClaimType);
                claims.Add(WLClaims.OwnAccount.CanDisable2FA.ClaimType);
                claims.Add(WLClaims.OwnAccount.CanEnable2FA.ClaimType);
                claims.Add(WLClaims.Administration.Account.CanResetPassword.ClaimType);
                claims.Add(WLClaims.Administration.Account.CanChangePassword.ClaimType);
                claims.Add(WLClaims.Administration.Account.CanDisable2FA.ClaimType);
                claims.Add(WLClaims.Administration.Account.CanEnable2FA.ClaimType);
            }

            var openId = options.IdentityResources["openid"];
            openId.UserClaims.Add(JwtClaimTypes.Id);
            openId.UserClaims.Add(JwtClaimTypes.Name);
            openId.UserClaims.Add(JwtClaimTypes.Role);
            AddWLClaims(openId.UserClaims);

            var api = options.ApiResources.Single();
            api.UserClaims.Add(JwtClaimTypes.Id);
            api.UserClaims.Add(JwtClaimTypes.Name);
            api.UserClaims.Add(JwtClaimTypes.Role);
            AddWLClaims(api.UserClaims);
        }

        private void ConfigureAuthorization(AuthorizationOptions options)
        {
            options.AddPolicy(WLPolicies.OwnAccount.Policies.CanLoginExternal)
                   .AddPolicy(WLPolicies.OwnAccount.Policies.CanResetPassword)
                   .AddPolicy(WLPolicies.OwnAccount.Policies.CanChangePassword)
                   .AddPolicy(WLPolicies.OwnAccount.Policies.CanDisable2FA)
                   .AddPolicy(WLPolicies.OwnAccount.Policies.CanEnable2FA);

            options.AddPolicy(WLPolicies.Administration.Account.Policies.CanResetPassword)
                   .AddPolicy(WLPolicies.Administration.Account.Policies.CanChangePassword)
                   .AddPolicy(WLPolicies.Administration.Account.Policies.CanDisable2FA)
                   .AddPolicy(WLPolicies.Administration.Account.Policies.CanEnable2FA);
        }

        #endregion
    }
}
