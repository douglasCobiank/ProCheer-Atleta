using System.Text;
using System.Text.Json;
using Atleta.Core.Models;
using Atleta.Infrastructure.Cache;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Atleta.Core.Services.Background
{
    public class WorkerAtualizaCacheHostedService : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private readonly IServiceProvider _serviceProvider;

        public WorkerAtualizaCacheHostedService(IServiceProvider serviceProvider)
        {
            _factory = new ConnectionFactory() { HostName = "localhost" }; // host do RabbitMQ
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var conexao = _factory.CreateConnection();
            var canal = conexao.CreateModel();

            canal.QueueDeclare(queue: "atualizar-cache", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumidor = new EventingBasicConsumer(canal);
            consumidor.Received += async (modelo, ea) =>
            {
                using var scope = _serviceProvider.CreateScope();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var atleta = JsonSerializer.Deserialize<AtletasDto>(json);

                if (atleta != null)
                {
                    var cacheKey = $"atleta_{atleta.Usuario.Nome?.ToLower()}";
                    await cacheService.SetAsync(cacheKey, atleta, TimeSpan.FromMinutes(10));
                    Console.WriteLine($"[WORKER] Cache atualizado para {atleta.Usuario.Nome}");
                }
            };

            canal.BasicConsume(queue: "atualizar-cache", autoAck: true, consumer: consumidor);

            Console.WriteLine("[WORKER] Worker iniciado. Aguardando mensagens...");
            return Task.CompletedTask;
        }
    }
}