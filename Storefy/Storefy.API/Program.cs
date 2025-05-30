using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Storefy.API.Handlers;
using Storefy.API.Middlewares;
using Storefy.Interfaces;
using Storefy.Interfaces.Services;
using Storefy.Interfaces.Services.Notifications;
using Storefy.Services;
using Storefy.Services.Data;
using Storefy.Services.Services;
using Storefy.Services.Services.Notifications;
using Swashbuckle.AspNetCore.Filters;

var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
        });
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/StorefyLogs.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddDbContext<StorefyDbContext>(options =>
                             options.UseSqlServer(builder.Configuration
                             .GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IPlatformService, PlatformService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();

builder.Services.AddSingleton<ITopicClient>(x =>
new TopicClient(
    builder.Configuration.GetSection("AzureServiceBus:ConnectionString").Value,
    builder.Configuration.GetSection("AzureServiceBus:TopicName").Value));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
        .GetBytes(builder.Configuration["JwtSettings:Key"])),
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "Admin",
        policy => policy.RequireClaim(ClaimTypes.Role, "Administrator"));
    options.AddPolicy(
        "Manager",
        policy => policy.RequireClaim(ClaimTypes.Role, "Manager", "Administrator"));
    options.AddPolicy(
        "Moderator",
        policy => policy.RequireClaim(ClaimTypes.Role, "Moderator", "Administrator", "Manager"));
    options.AddPolicy(
        "User",
        policy => policy.RequireClaim(ClaimTypes.Role, "User", "Moderator", "Administrator", "Manager"));
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionHandler>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

app.UseRouting();

app.UseMiddleware<GameCountMiddleware>();
app.UseMiddleware<LoggingIpAddressMiddleware>();
app.UseMiddleware<ElapsedTimeLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(myAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
