using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using DeliveryAPI;
using DeliveryAPI.Authorization;
using DeliveryAPI.Configurations;
using DeliveryAPI.DbContext;
using DeliveryAPI.Helpers;
using DeliveryAPI.Middleware;
using DeliveryAPI.Models;
using DeliveryAPI.Services.BasketServices;
using DeliveryAPI.Services.AddressService;
using DeliveryAPI.Services.DishService;
using DeliveryAPI.Services.JwtService;
using DeliveryAPI.Services.OrderService;
using DeliveryAPI.Services.Repository;
using DeliveryAPI.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    options.OperationFilter<SwaggerFilter>();
});

var connection = builder.Configuration.GetConnectionString("Postgres");
builder.Services.AddDbContext<ApplicationDbContexts>(options => options.UseNpgsql(connection));
builder.Services.AddDbContext<DeliveryContext>(options => options.UseNpgsql(connection));

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = JwtConfigurations.Issuer,
            ValidAudience = JwtConfigurations.Audience,
            IssuerSigningKey = JwtConfigurations.GetSymmetricSecurityKey(),
        };
        
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "AuthorizationPolicy",
        policy => policy.Requirements.Add(new AuthorizationRequirements()));
});
builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
builder.Services
    .AddControllers()
    .AddJsonOptions(options => 
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IDishService, DishServices>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRepositoryService, RepositoryService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddHostedService<CleanExpiredTokens>();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContexts>();
dbContext?.Database.Migrate();

app.UseMiddleware<MiddlewareHandler>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();