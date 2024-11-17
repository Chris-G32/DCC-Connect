using API.Models;
using API.Services;
using Carter;
using MongoDB.Bson;
using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "API", Version = "v1" });

    // Add security definition
    c.AddSecurityDefinition("basic", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Enter your credentials to access the Swagger UI"
    });

    // Add security requirement
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        { new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            }, new string[] { }
        }
    });
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
builder.Services.AddSingleton<IDBClientProvider, MongoClientProvider>();
builder.Services.AddSingleton<IDatabaseProvider, DatabaseProvider>();
builder.Services.AddSingleton<ICollectionsProvider, CollectionsProvider>();
builder.Services.AddSingleton<IAvailabiltyService, AvailablityService>();
builder.Services.AddSingleton<IShiftScheduler, ShiftScheduler>();
builder.Services.AddSingleton<IShiftRetriever, ShiftRetriever>();
builder.Services.AddSingleton<IShiftTrader, ShiftTrader>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapCarter();

app.Run();
