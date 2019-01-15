using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Job.BlockchainRiskControl.Contract.Commands;
using Lykke.Job.BlockchainRiskControl.Contract.Events;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Services;
using Lykke.Job.BlockchainRiskControl.Mapping;
using Lykke.Job.BlockchainRiskControl.Workflow.Events;

namespace Lykke.Job.BlockchainRiskControl.Workflow.CommandHandlers
{
    [UsedImplicitly]
    public class ValidateOperationCommandsHandler
    {
        private readonly IOperationValidationService _operationValidationService;

        public ValidateOperationCommandsHandler(IOperationValidationService operationValidationService)
        {
            _operationValidationService = operationValidationService;
        }

        [UsedImplicitly]
        public async Task<CommandHandlingResult> Handle(ValidateOperationCommand command, IEventPublisher publisher)
        {
            var operation = new Operation
            (
                id: command.OperationId,
                type: command.Type.ToDomain(),
                userId: command.UserId,
                blockchainType: command.BlockchainType,
                blockchainAssetId: command.BlockchainAssetId,
                fromAddress: command.FromAddress,
                toAddress: command.ToAddress,
                amount: command.Amount
            );

            var validation = await _operationValidationService.ValidateAsync(operation);

            if (validation.Risk.Level == OperationRiskLevel.Reliable)
            {
                publisher.PublishEvent
                (
                    new OperationAcceptedEvent
                    {
                        OperationId = validation.Operation.Id
                    }
                );
            }
            else
            {
                publisher.PublishEvent
                (
                    new RiskyOperationDetectedEvent
                    {
                        OperationId = validation.Operation.Id,
                        RiskLevel = validation.Risk.Level
                    }
                );
            }

            return CommandHandlingResult.Ok();
        }
    }
}
