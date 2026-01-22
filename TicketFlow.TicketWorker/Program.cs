using TicketFlow.Api; 
using TicketFlow.TicketWorker;
using TicketFlow.TicketWorker.Services;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, o => o.UseVector()));

builder.Services.AddScoped<ITicketProcessor, TicketProcessor>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();