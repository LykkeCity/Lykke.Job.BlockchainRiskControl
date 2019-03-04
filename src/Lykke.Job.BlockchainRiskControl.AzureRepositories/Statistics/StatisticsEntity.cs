using System;
using Lykke.Job.BlockchainRiskControl.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Statistics
{
    internal class StatisticsEntity
    {
        public Guid Id { get; set; }
        public OperationType Type { get; set; }
        public Guid UserId { get; set; }
        public string BlockchainType { get; set; }
        public string BlockchainAssetId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }

        public StatisticsEntity()
        {
            // default ctor for deserialization
        }
        
        public StatisticsEntity(Operation operation)
        {
            Id = operation.Id;
            Type = operation.Type;
            UserId = operation.UserId;
            BlockchainType = operation.BlockchainType;
            BlockchainAssetId = operation.BlockchainAssetId;
            FromAddress = operation.FromAddress;
            ToAddress = operation.ToAddress;
            Amount = operation.Amount;
            Timestamp = DateTime.UtcNow;
        }
    }
}
