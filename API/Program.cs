using API.Models;
using API.Services;
using API.Services.QueryExecuters;
using Carter;
using MongoDB.Bson;
using MongoDB.Driver;

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

/* Auth Services */
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IJwtReaderService, JwtReaderService>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IUserService, UserService>();

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
app.MapCarter();

app.Run();
