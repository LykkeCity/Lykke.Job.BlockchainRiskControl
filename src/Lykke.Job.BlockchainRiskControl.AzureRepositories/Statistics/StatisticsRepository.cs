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

        public StatisticsRepository(string connectionString, ILogFactory logFactory)
        {
            var url = new MongoUrl(connectionString);

            _db = new MongoClient(url).GetDatabase(
                url.DatabaseName ?? "BlockchainRiskControl",
                new MongoDatabaseSettings
                {
                    GuidRepresentation = GuidRepresentation.Standard
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

        public async Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(TimeSpan period)
        {
            return await Statistics.Aggregate()
                .Match(e => e.Timestamp >= DateTime.UtcNow.Subtract(period))
                .Group(e => 1, g => g.Sum(e => e.Amount))
                .SingleAsync();
        }

        public async Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(TimeSpan period, Guid userId)
        {
            return await Statistics.Aggregate()
                .Match(e => e.Timestamp >= DateTime.UtcNow.Subtract(period) && e.UserId == userId)
                .Group(e => 1, g => g.Sum(e => e.Amount))
                .SingleAsync();
        }

        public async Task<long> GetOperationsCountForTheLastPeriodAsync(TimeSpan period)
        {
            return await Statistics.CountDocumentsAsync(e => e.Timestamp >= DateTime.UtcNow.Subtract(period));
        }

        public async Task<long> GetOperationsCountForTheLastPeriodAsync(TimeSpan period, Guid userId)
        {
            return await Statistics.CountDocumentsAsync(e => e.Timestamp >= DateTime.UtcNow.Subtract(period) && e.UserId == userId);
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