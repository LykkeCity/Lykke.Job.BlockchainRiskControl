using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

        public WaitForOperationResolutionCommandsHandler(
            TimeSpan checkingPeriod,
            IOperationValidationService operationValidationService)
        {
            _checkingPeriod = checkingPeriod;
            _operationValidationService = operationValidationService;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(WaitForOperationResolutionCommand command, IEventPublisher publisher)
        {
            var resolution = await _operationValidationService.WaitForResolutionAsync(command.OperationId);

            switch (resolution)
            {
                case OperationValidationResolution.Unconfirmed:
                    return CommandHandlingResult.Fail(_checkingPeriod);

                case OperationValidationResolution.Accepted:
                    publisher.PublishEvent
                    (
                        new OperationAcceptedEvent
                        {
                            OperationId = command.OperationId
                        }
                    );
                    break;

                case OperationValidationResolution.Rejected:
                    publisher.PublishEvent
                    (
                        new OperationRejectedEvent
                        {
                            OperationId = command.OperationId
                        }
                    );
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }
            
            return CommandHandlingResult.Ok();
        }
    }
}
