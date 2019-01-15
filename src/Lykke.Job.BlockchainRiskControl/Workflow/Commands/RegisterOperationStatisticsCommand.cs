using System;
using MessagePack;

namespace Lykke.Job.BlockchainRiskControl.Workflow.Commands
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RegisterOperationStatisticsCommand
    {
        public Guid OperationId { get; set; }
    }
}
