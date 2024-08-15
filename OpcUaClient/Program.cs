using System.Text.Json.Serialization;
using OpcUaClient.Options;
using OpcUaClient.Services;
using OpcUaClient.Services.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitSettings = builder.Configuration.GetSection("RabbitMQ");
var options = rabbitSettings.Get<RabbitSettings>();
builder.Services.Configure<RabbitSettings>(rabbitSettings);
builder.Services.AddOptions<RabbitMqTransportOptions>()
    .Configure(o =>
    {
        o.Host = options?.Server;
        o.VHost = options?.VirtualHost;
        o.User = options?.User;
        o.Pass = options?.Password;
    });
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq();
});

builder.Services.AddSingleton<IOpcUaService, OpcUaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();