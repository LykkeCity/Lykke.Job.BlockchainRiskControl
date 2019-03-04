using System;

namespace Lykke.Job.BlockchainRiskControl.Domain
{
    public sealed class Operation
    {
        public Guid Id { get; }
        public OperationType Type { get; }
        public Guid UserId { get; }
        public string BlockchainType { get; }
        public string BlockchainAssetId { get; }
        public string FromAddress { get; }
        public string ToAddress { get; }
        public decimal Amount { get; }

        public Operation(
            Guid id,
            OperationType type,
            Guid userId,
            string blockchainType,
            string blockchainAssetId,
            string fromAddress,
            string toAddress,
            decimal amount)
        {
            Id = id;
            Type = type;
            UserId = userId;
            BlockchainType = blockchainType;
            BlockchainAssetId = blockchainAssetId;
            FromAddress = fromAddress;
            ToAddress = toAddress;
            Amount = amount;
        }
    }
}
