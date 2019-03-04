using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Job.BlockchainRiskControl.Contract;
using Lykke.Job.BlockchainRiskControl.Contract.Events;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Workflow.Commands;
using Lykke.Job.BlockchainRiskControl.Workflow.Events;

namespace Lykke.Job.BlockchainRiskControl.Workflow.Sagas
{
    public class OperationValidationSaga
    {
        [UsedImplicitly]
        public Task Handle(RiskyOperationDetectedEvent evt, ICommandSender sender)
        {
            switch (evt.RiskLevel)
            {
                case OperationRiskLevel.ResolutionRequired:
                    sender.SendCommand
                    (
                        new WaitForOperationResolutionCommand
                        {
                            OperationId = evt.OperationId
                        },
                        BlockchainRiskControlBoundedContext.Name
                    );
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(evt.RiskLevel), evt.RiskLevel, null);
            }

            return Task.CompletedTask;
        }

        [UsedImplicitly]
        public Task Handle(OperationAcceptedEvent evt, ICommandSender sender)
        {
            sender.SendCommand
            (
                new RegisterOperationStatisticsCommand
                {
                    OperationId = evt.OperationId
                },
                BlockchainRiskControlBoundedContext.Name
            );

            return Task.CompletedTask;
        }
    }
}
