using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoreHub.Domain.Entities;
using StoreHub.Infrastructure.Data;
using StoreHub.API.Mappings;
using StoreHub.Application.Mappings;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
            uri.Scheme == "http" &&
            uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token as: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddApplicationServices();

builder.Services.AddAutoMapper(typeof(MappingProfile), typeof(ApiMappingProfile));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ReactPolicy");

app.UseStaticFiles();

// app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

await SeedAdminUserAsync(app);

app.Run();

static async Task SeedAdminUserAsync(WebApplication app)
{
    var adminEmail = app.Configuration["AdminSeed:Email"]?.Trim().ToLowerInvariant();
    var adminPassword = app.Configuration["AdminSeed:Password"]?.Trim();
    var adminFullName = app.Configuration["AdminSeed:FullName"]?.Trim();

    if (string.IsNullOrWhiteSpace(adminEmail) ||
        string.IsNullOrWhiteSpace(adminPassword) ||
        string.IsNullOrWhiteSpace(adminFullName))
    {
        return;
    }

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var existingAdmin = await dbContext.Users
        .FirstOrDefaultAsync(user => user.Email.ToLower() == adminEmail);

    if (existingAdmin == null)
    {
        await dbContext.Users.AddAsync(new User
        {
            Id = Guid.NewGuid(),
            FullName = adminFullName,
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            IsAdmin = true,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
        return;
    }

    var updated = false;

    if (!existingAdmin.IsAdmin)
    {
        existingAdmin.IsAdmin = true;
        updated = true;
    }

    if (!existingAdmin.IsActive)
    {
        existingAdmin.IsActive = true;
        updated = true;
    }

    // Migrate legacy plain-text password to BCrypt for seeded admin.
    if (!existingAdmin.PasswordHash.StartsWith("$2") && existingAdmin.PasswordHash == adminPassword)
    {
        existingAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword);
        updated = true;
    }

    if (updated)
    {
        existingAdmin.UpdatedDate = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }
}