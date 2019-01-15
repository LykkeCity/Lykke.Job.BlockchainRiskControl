using System;
using Lykke.Job.BlockchainRiskControl.Domain;
using MessagePack;

namespace Lykke.Job.BlockchainRiskControl.Workflow.Events
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class RiskyOperationDetectedEvent
    {
        public Guid OperationId { get; set; }
        public OperationRiskLevel RiskLevel { get; set; }
    }
}
