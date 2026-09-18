using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AgendaaS.Application.Interfaces;
using AgendaaS.Application.Services;
using AgendaaS.Infrastructure.Data;
using AgendaaS.Infrastructure.Services;
using AgendaaS.Application.Settings;
using AgendaaS.Api.Middleware;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITenantProvider, CurrentTenantService>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());

builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgendaaS API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection(); // dentro do container não há certificado HTTPS configurado
}

app.UseCors("AllowFrontend");
app.UseAuthentication();

app.Use(async (context, next) =>
{
    var tenantProvider = context.RequestServices.GetRequiredService<ITenantProvider>();

    if (context.Request.Path.StartsWithSegments("/api/public", out var remaining))
    {
        var slug = remaining.Value?.Trim('/').Split('/').FirstOrDefault();
        if (!string.IsNullOrEmpty(slug))
        {
            var db = context.RequestServices.GetRequiredService<IApplicationDbContext>();
            var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Slug == slug);
            if (tenant != null)
                tenantProvider.SetTenantId(tenant.Id);
        }
    }
    else if (context.User.Identity?.IsAuthenticated == true)
    {
        var claim = context.User.FindFirst("tenant_id")?.Value;
        if (Guid.TryParse(claim, out var tenantId))
            tenantProvider.SetTenantId(tenantId);
    }

    await next();
});

app.UseAuthorization();
app.MapControllers();
app.Run();
