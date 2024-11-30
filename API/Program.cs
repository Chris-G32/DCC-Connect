using API.Constants;
using API.Models;
using API.Services;
using API.Services.Authentication;
using API.Services.QueryExecuters;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API", Version = "v1" });

    // Add JWT Bearer authentication definition
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token. Example: 'Bearer eyJhbGciOiJIUzI1...'"
    });

    // Add security requirement
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .SetIsOriginAllowed((host) => true)
        .AllowCredentials());
});

builder.Services.AddLogging();
builder.Services.AddCarter();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new ArgumentNullException("JWT:Secret must be set in secrets.json");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Configure in appsettings.json
            ValidAudience = builder.Configuration["Jwt:Audience"], // Configure in appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)) // Configure in secrets.json
        };

        // Extract JWT from cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("sessionid", out var token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyConstants.EmployeePolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy(PolicyConstants.ManagerPolicy, policy =>
    {
        policy.RequireRole(RoleConstants.Manager, RoleConstants.Admin);
    });
    options.AddPolicy(PolicyConstants.AdminPolicy, policy =>
    {
        policy.RequireRole(RoleConstants.Admin);
    });
});

/* Auth Services */
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IUserRegisterService, UserRegisterService>();

/* Mongo and Shift Services */
builder.Services.AddSingleton<IMongoDBSettingsProvider, MongoDBSettingsProvider>();
builder.Services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();
builder.Services.AddSingleton<ICollectionsProvider, CollectionsProvider>();
builder.Services.AddSingleton<IEntityRetriever, EntityRetriever>();
builder.Services.AddSingleton<IDBClientProvider, MongoClientProvider>();
builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>();
builder.Services.AddSingleton<IAvailabiltyService, AvailablityService>();
builder.Services.AddSingleton<IShiftScheduler, ShiftScheduler>();
builder.Services.AddSingleton<IShiftQueryExecuter, ShiftQueryExecuter>();
builder.Services.AddSingleton<IEmployeeQueryExecuter, EmployeeQueryExecuter>();
builder.Services.AddSingleton<ICoverageRequestQueryExecuter, CoverageRequestQueryExecuter>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<ILoginService, LoginService>();
builder.Services.AddSingleton<IMFAService, MFAService>();
builder.Services.AddSingleton<IShiftTrader, ShiftTrader>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.Logger.LogDebug("In Development environment");
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization(); // Enable authorization middleware
app.MapCarter();
app.Run();
