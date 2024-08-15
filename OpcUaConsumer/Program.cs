using System.Reflection;
using System.Text.Json.Serialization;
using MassTransit;
using OpcUaConsumer.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Configuration
    .AddJsonFile("appsettings.json");

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
    var entryAssembly = Assembly.GetExecutingAssembly();
    x.AddConsumers(entryAssembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();