
using System.CodeDom;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using MessagingServiceBlazorApp.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Threading;

namespace MessagingServiceBlazorApp.Features.Messaging.Services
{
    public class MessagingBrokerClient : IMessagingBrokerClient
    {
        private readonly ILogger<MessagingBrokerClient> _logger;
        private readonly MessagingSettings? _configuration;
        private ProducerConfig _producerConfig = default!;

        public event AsyncEventHandler<string>? MessageConsumed;
        private CancellationToken listeningToken = new CancellationToken();
        private readonly ConsumerConfig _consumerConfig;

        /// <summary>
        /// MessagingBrokerClient.
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="configuration">configuration</param>
        public MessagingBrokerClient(
            IOptions<MessagingSettings> configuration,
            ILogger<MessagingBrokerClient> logger
            )
        {
            _logger = logger;
            _configuration = configuration.Value;
            _producerConfig = KafkaProducerConfig();
            _consumerConfig = KafkaConsumerConfig();
        }


        public async Task<string> PublishAsync(string topic, string message)
        {
            try
            {
                using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();

                var result = await producer.ProduceAsync(topic,
                        new Message<Null, string> { Value = message! });

                return result.Message.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred: {ex}");
                throw;
            }
        }

        public async Task StartListeningAsync(string topicName, CancellationToken cancellationToken)
        {
            MessageConsumed?.InvokeAsync(this, "start listening...");

            await Task.Run(() =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
                consumer.Subscribe(topicName);
                while (!cancellationToken.IsCancellationRequested && !listeningToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume();
                    MessageConsumed?.InvokeAsync(this, consumeResult.Message.Value!);
                }
                consumer.Close();
            });
        }

        public void StopListening(CancellationToken cancellationToken)
        {
            listeningToken = cancellationToken;
        }

        private ProducerConfig KafkaProducerConfig()
        {
            if (Enum.TryParse<SecurityProtocol>(_configuration?.KafkaSettings?.SecurityProtocol, out SecurityProtocol securityProtocol)
                    &&
                Enum.TryParse<SaslMechanism>(_configuration?.KafkaSettings?.SaslMechanism, out SaslMechanism saslMechanism))
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = _configuration?.KafkaSettings?.BootstrapServers,
                    SecurityProtocol = securityProtocol,
                    SaslMechanism = saslMechanism,
                    SaslUsername = _configuration?.KafkaSettings?.SaslUsername,
                    SaslPassword = _configuration?.KafkaSettings?.SaslPassword,
                };
                return producerConfig;
            }
            throw new Exception("some configuration parameters cannot be parsed");
        }

        private ConsumerConfig KafkaConsumerConfig()
        {
            if (Enum.TryParse<SecurityProtocol>(_configuration?.KafkaSettings?.SecurityProtocol, out SecurityProtocol securityProtocol)
                    &&
                Enum.TryParse<SaslMechanism>(_configuration?.KafkaSettings?.SaslMechanism, out SaslMechanism saslMechanism))
            {
                var producerConfig = new ConsumerConfig
                {
                    BootstrapServers = _configuration?.KafkaSettings?.BootstrapServers,
                    SecurityProtocol = securityProtocol,
                    SaslMechanism = saslMechanism,
                    SaslUsername = _configuration?.KafkaSettings?.SaslUsername,
                    SaslPassword = _configuration?.KafkaSettings?.SaslPassword,
                    GroupId = "web-blazor-app-v1"
                };
                return producerConfig;
            }
            throw new Exception("some configuration parameters cannot be parsed");
        }        
    }
}
