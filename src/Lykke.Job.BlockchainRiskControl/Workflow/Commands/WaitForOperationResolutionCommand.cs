using System;
using MessagePack;

namespace Lykke.Job.BlockchainRiskControl.Workflow.Commands
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class WaitForOperationResolutionCommand
    {
        public Guid OperationId { get; set; }
    }
}
