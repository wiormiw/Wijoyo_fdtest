using Wijoyo_fdtest.Application.Common.Interfaces;
using Wijoyo_fdtest.Domain.Constants;
using Wijoyo_fdtest.Infrastructure.Data;
using Wijoyo_fdtest.Infrastructure.Data.Interceptors;
using Wijoyo_fdtest.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Wijoyo_fdtest.Infrastructure.Storage;
using Wijoyo_fdtest.Infrastructure.Jwt;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Wijoyo_fdtest");
        Guard.Against.Null(connectionString, message: "Connection string 'Wijoyo_fdtest' DB not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString).AddAsyncSeeding(sp);
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        // MinIO
        builder.Services.AddTransient<IStorageService, MinioStorageService>();
        builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("MinIO"));

        // Auth
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        
        // Email Verification
        builder.Services.AddScoped<IVerificationTokenService, VerificationTokenService>();
    }
}
