using System;
using Lykke.Job.BlockchainRiskControl.Domain;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Validation
{
    internal class OperationEntity
    {
        public Guid Id { get; set; }
        public OperationType Type { get; set; }
        public Guid UserId { get; set; }
        public string BlockchainType { get; set; }
        public string BlockchainAssetId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }

        public OperationEntity()
        {
            // default ctor for deserialization
        }

        public OperationEntity(Operation operation)
        {
            Id = operation.Id;
            Type = operation.Type;
            UserId = operation.UserId;
            BlockchainType = operation.BlockchainType;
            BlockchainAssetId = operation.BlockchainAssetId;
            FromAddress = operation.FromAddress;
            ToAddress = operation.ToAddress;
            Amount = operation.Amount;
        }

        public Operation ToOperation()
        {
            return new Operation(Id, Type, UserId, BlockchainType, BlockchainAssetId, FromAddress, ToAddress, Amount);
        }
    }
}