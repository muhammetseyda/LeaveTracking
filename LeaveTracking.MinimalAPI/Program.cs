using FluentValidation;
using LeaveTracking.Application.DTOs.LeaveRequestDTOs;
using LeaveTracking.Application.DTOs.UserDTOs;
using LeaveTracking.Application.Interfaces.Repositories;
using LeaveTracking.Application.Interfaces.Services;
using LeaveTracking.Application.Mapper;
using LeaveTracking.Application.Services;
using LeaveTracking.Infrastructure.Persistence.Context;
using LeaveTracking.Infrastructure.Repositories;
using LeaveTracking.Infrastructure.Seed;
using LeaveTracking.Infrastructure.Services;
using LeaveTracking.MinimalAPI;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", opt =>
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RoleClaimType = ClaimTypes.Role,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireManager", policy => policy.RequireRole("Manager"));
});

builder.Services.AddAutoMapper(builder =>
{
    builder.AddProfile<MappingProfile>();
}, typeof(Program).Assembly);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDto>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateLeaveRequestDto>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var relativePath = Path.Combine(env.ContentRootPath, "..", "LeaveTracking.Infrastructure", "Externals", "EmailTemplates");
    var fullPath = Path.GetFullPath(relativePath);
    return new EmailTemplateService(fullPath);
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("sakin", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 30;
        limiterOptions.QueueLimit = 5;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    context.Database.Migrate(); 
    await SeedData.SeedAsync(context);
}

app.MapScalarApiReference(
    opt =>
    {
        opt.Title = "Leave Tracking";
        opt.Theme = ScalarTheme.Default;
        opt.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11);
    });
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapManagerEndpoints();

app.Run();