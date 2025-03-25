using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Nest;
using Products.Data;
using Products.Models;

var builder = WebApplication.CreateBuilder(args);
string configFileName = "appsettings.json";
var configuration = new ConfigurationBuilder().SetBasePath(builder.Environment.ContentRootPath).AddJsonFile(configFileName).Build();

builder.Services.AddSingleton(configuration);

var DatabaseName = configuration.GetSection("CosmosDBSettings:DatabaseName").Value;

var PrimaryKey = configuration.GetSection("CosmosDBSettings:PrimaryKey").Value;

var CosmosDbURL = configuration.GetSection("CosmosDBSettings:CosmosDbURL").Value;

//var DatabaseName = configuration.GetValue<string>("CosmosDBSettings:DatabaseName");

//var PrimaryKey = configuration.GetSection("CosmosDBSettings:PrimaryKey").Value;

//var CosmosDbURL = configuration.GetSection("CosmosDBSettings:CosmosDbURL").Value;

builder.Services.AddDbContext<ProductsDbContext>(options => options.UseCosmos(CosmosDbURL, PrimaryKey, DatabaseName));

builder.Services.AddSingleton<CosmosClient>(new CosmosClient(CosmosDbURL, PrimaryKey));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

//builder.SetupElastic(configuration);

var app = builder.Build();

app.CreateDBIfNotAlready();

//app.SynchroniseElasticData(configuration);

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();


