using System.Text.Json.Serialization;
using OpcUaClient.Options;
using OpcUaClient.Services;
using OpcUaClient.Services.Interfaces;
using MassTransit;
using OpcUaClient.Extensions;
using OpcUaClient.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDataAccessLayer(builder.Configuration);

const string origin = "MyAllowSpecificOrigins";
builder.Services.AddCorsPolicy(builder.Configuration, origin);

builder.Services.AddSerilogLogging();
builder.Services.AddFluentValidation();
builder.Services.AddAutoMapper(typeof(Program).Assembly);


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
app.UseCors(origin);

app.UseSwagger();
app.UseSwaggerUI();

app.UseCustomLoggingHandler();
app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();