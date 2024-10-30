using FileCreateWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMqWeb.ExcelCreate.Services;

namespace FileCreateWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddSingleton(sp => new ConnectionFactory() { Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")), DispatchConsumersAsync = true });
            builder.Services.AddSingleton<RabbitMqClientService>();
            builder.Services.AddHostedService<Worker>();
            builder.Services.AddDbContext<AdventureWorks2012Context>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });
            var host = builder.Build();
            host.Run();
        }
    }
}