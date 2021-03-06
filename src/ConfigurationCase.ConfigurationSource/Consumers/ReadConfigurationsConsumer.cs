using Configuration.Core.Events;
using ConfigurationCase.ConfigurationSource.Abstracts;
using MassTransit;
using System.Threading.Tasks;

namespace ConfigurationCase.ConfigurationSource.Consumers
{
    public class ReadConfigurationsConsumer : IConsumer<RequestConfigurationEvent>
    {
        private readonly IConfigurationJobService _service;

        public ReadConfigurationsConsumer(IConfigurationJobService service)
        {
            _service = service;
        }

        public async Task Consume(ConsumeContext<RequestConfigurationEvent> context)
        {
            await _service.ReadConfigurationAsync(context.Message.ApplicationName, context.Message.ConnectionString, context.Message.RefreshTimerIntervalInMs);
        }
    }
}
