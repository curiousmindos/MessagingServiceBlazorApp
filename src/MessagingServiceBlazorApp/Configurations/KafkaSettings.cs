using Confluent.Kafka;

namespace MessagingServiceBlazorApp.Configurations
{
    public class KafkaSettings
    {
        public string? BootstrapServers { get; set; }

        public string SecurityProtocol { get; set; } = default!;

        public string? SaslMechanism { get; set; } = default!;

        public string? SaslUsername { get; set; }

        public string? SaslPassword { get; set; }

        public string? SslCaLocation { get; set; }

        public SchemaRegistry? SchemaRegistry { get; set; }
    }

    public class SchemaRegistry
    {
        public string? Url { get; set; }
        public string? AuthUserInfo { get; set; }
    }
}
