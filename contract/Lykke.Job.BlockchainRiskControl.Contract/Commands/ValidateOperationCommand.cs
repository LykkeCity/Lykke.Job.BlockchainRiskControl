using System;
using JetBrains.Annotations;
using MessagePack;

namespace Lykke.Job.BlockchainRiskControl.Contract.Commands
{
    [PublicAPI]
    [MessagePackObject(keyAsPropertyName: true)]
    public class ValidateOperationCommand
    {
        public Guid OperationId { get; set; }
        public OperationType Type { get; set; }
        public Guid UserId { get; set; }
        public string BlockchainType { get; set; }
        public string BlockchainAssetId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get;set; }
    }
}
