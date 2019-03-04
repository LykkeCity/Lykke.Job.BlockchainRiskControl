using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Job.BlockchainRiskControl.Domain.Services;
using Lykke.Job.BlockchainRiskControl.Workflow.Commands;

namespace Lykke.Job.BlockchainRiskControl.Workflow.CommandHandlers
{
    [UsedImplicitly]
    public class RegisterOperationStatisticsCommandsHandler
    {
        private readonly IStatisticsService _statisticsService;

        public RegisterOperationStatisticsCommandsHandler(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(RegisterOperationStatisticsCommand command, IEventPublisher publisher)
        {
            await _statisticsService.RegisterStatisticsAsync(command.OperationId);

            return CommandHandlingResult.Ok();
        }
    }
}
