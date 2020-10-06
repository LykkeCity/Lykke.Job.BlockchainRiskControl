using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainRiskControl.Contract.Events;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Services;
using Lykke.Job.BlockchainRiskControl.Workflow.Commands;

namespace Lykke.Job.BlockchainRiskControl.Workflow.CommandHandlers
{
    [UsedImplicitly]
    public class WaitForOperationResolutionCommandsHandler
    {
        private readonly TimeSpan _checkingPeriod;
        private readonly IOperationValidationService _operationValidationService;
        private readonly ILog _log;

        public WaitForOperationResolutionCommandsHandler(
            ILogFactory loggerFactory,
            TimeSpan checkingPeriod,
            IOperationValidationService operationValidationService)
        {
            _log = loggerFactory.CreateLog(this);
            _checkingPeriod = checkingPeriod;
            _operationValidationService = operationValidationService;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(WaitForOperationResolutionCommand command, IEventPublisher publisher)
        {
            var resolution = await _operationValidationService.GetResolutionAsync(command.OperationId);

            switch (resolution)
            {
                case OperationValidationResolution.Unconfirmed:
                    _log.Warning("Operation requires for manual confirmation", context: command);
                    return CommandHandlingResult.Fail(_checkingPeriod);

                case OperationValidationResolution.Accepted:
                case OperationValidationResolution.Rejected:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }

            return CommandHandlingResult.Ok();
        }
    }
}
