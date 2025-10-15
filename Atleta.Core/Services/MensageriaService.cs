using System.Text;
using System.Text.Json;
using Atleta.Core.Interface;
using RabbitMQ.Client;

namespace Atleta.Core.Services
{
    public class MensageriaService : IMensageriaService
    {
        private readonly ConnectionFactory _factory;

        public MensageriaService()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost", // altere se o RabbitMQ estiver em outro host
                DispatchConsumersAsync = true // necessário se usar consumers assíncronos
            };
        }
    
        public void PublicarMensagem<T>(string fila, T mensagem)
        {
            try
            {
                using var conexao = _factory.CreateConnection();
                using var canal = conexao.CreateModel();

                // Declara a fila caso ainda não exista
                canal.QueueDeclare(
                    queue: fila,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                // Serializa a mensagem
                var corpo = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));

                // Publica a mensagem
                canal.BasicPublish(
                    exchange: "",
                    routingKey: fila,
                    basicProperties: null,
                    body: corpo);

                Console.WriteLine($"[RABBITMQ] Mensagem publicada na fila '{fila}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RABBITMQ] Erro ao publicar mensagem: {ex.Message}");
                throw;
            }
        
        }
    }
}