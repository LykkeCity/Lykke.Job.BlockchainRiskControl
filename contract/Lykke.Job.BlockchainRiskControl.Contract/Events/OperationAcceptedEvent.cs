using System;
using JetBrains.Annotations;
using MessagePack;

namespace Lykke.Job.BlockchainRiskControl.Contract.Events
{
    [PublicAPI]
    [MessagePackObject(keyAsPropertyName: true)]
    public class OperationAcceptedEvent
    {
        public Guid OperationId { get; set; }
    }
}
