using API.Models;
using API.Services;
using Carter;
using MongoDB.Bson;
using MongoDB.Driver;
using API.Database;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();
builder.Services.AddSingleton<IDatabaseProvider,DatabaseProvider>();
builder.Services.AddSingleton<ICollectionsProvider, CollectionsProvider>();
builder.Services.AddSingleton<IShiftScheduler, ShiftScheduler>();
builder.Services.AddSingleton<ICollectionsProvider, CollectionsProvider>();
var app = builder.Build();

MongoDBInitializer initializer = new(app.Configuration);
initializer.Initialize();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapCarter();

app.Run();
