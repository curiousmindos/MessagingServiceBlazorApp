using Microsoft.VisualStudio.Threading;

namespace MessagingServiceBlazorApp.Features.Messaging.Services
{
    public interface IMessagingBrokerClient
    {
        public Task<string> PublishAsync(string topic, string message);
        
        public event AsyncEventHandler<string> MessageConsumed;
        public Task StartListeningAsync(string topicName, CancellationToken cancellationToken);
        public void StopListening(CancellationToken cancellationToken);
    }
}
