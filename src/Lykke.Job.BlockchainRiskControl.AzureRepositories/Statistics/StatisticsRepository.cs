using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Statistics
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly IMongoDatabase _db;
        private readonly ILog _log;

        private IMongoCollection<StatisticsEntity> Statistics => _db.GetCollection<StatisticsEntity>("Statistics");

        private IAggregateFluent<StatisticsEntity> BuildQuery(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType)
        {
            var query = Statistics.Aggregate();

            if (!string.IsNullOrEmpty(blockchainType))
            {
                query = query.Match(e => e.BlockchainType == blockchainType);
            }

            if (!string.IsNullOrEmpty(blockchainAssetId))
            {
                query = query.Match(e => e.BlockchainAssetId == blockchainAssetId);
            }

            if (operationType.HasValue)
            {
                query = query.Match(e => e.Type == operationType);
            }

            return query;
        }

        public StatisticsRepository(string connectionString, ILogFactory logFactory)
        {
            var url = new MongoUrl(connectionString);

            _db = new MongoClient(url).GetDatabase(
                url.DatabaseName ?? "BlockchainRiskControl",
                new MongoDatabaseSettings
                {
                    GuidRepresentation = GuidRepresentation.Standard,
                });

            _log = logFactory.CreateLog(this);
        }

        public async Task CheckIndexesAsync()
        {
            var indexed = (await Statistics.Indexes.List().ToListAsync())
                .SelectMany(doc => doc["key"].AsBsonDocument.Names)
                .ToHashSet();

            var toIndex = new HashSet<string>()
            {
                nameof(StatisticsEntity.Timestamp),
                nameof(StatisticsEntity.UserId)
            };

            toIndex.ExceptWith(indexed);

            if (toIndex.Any())
            {
                _log.Warning($"Fields [{string.Join(", ", toIndex)}] are not indexed. Consider adding new index(es) to improve performance.");
            }
        }

        public async Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period)
        {
            var result = await BuildQuery(blockchainType, blockchainAssetId, operationType)
                .Match(e => e.Timestamp >= DateTime.UtcNow.Subtract(period))
                .Group(e => 1, g => new { Amount = g.Sum(e => e.Amount) })
                .SingleAsync();

            return result.Amount;
        }

        public async Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period,
            Guid userId)
        {
            var result = await BuildQuery(blockchainType, blockchainAssetId, operationType)
                .Match(e => e.Timestamp >= DateTime.UtcNow.Subtract(period) && e.UserId == userId)
                .Group(e => 1, g => new { Amount = g.Sum(e => e.Amount) })
                .SingleAsync();

            return result.Amount;
        }

        public async Task<long> GetOperationsCountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period)
        {
            var result = await BuildQuery(blockchainType, blockchainAssetId, operationType)
                .Match(e => e.Timestamp >= DateTime.UtcNow.Subtract(period))
                .Count()
                .SingleAsync();

            return result.Count;
        }

        public async Task<long> GetOperationsCountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period,
            Guid userId)
        {
            var result = await BuildQuery(blockchainType, blockchainAssetId, operationType)
                .Match(e => e.Timestamp >= DateTime.UtcNow.Subtract(period) && e.UserId == userId)
                .Count()
                .SingleAsync();

            return result.Count;
        }

        public async Task RegisterOperationAsync(Operation operation)
        {
            await Statistics.ReplaceOneAsync(
                e => e.Id == operation.Id,
                new StatisticsEntity(operation),
                new UpdateOptions
                {
                    IsUpsert = true,
                });
        }
    }
}