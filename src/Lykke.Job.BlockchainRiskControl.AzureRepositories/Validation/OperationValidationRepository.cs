using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Lykke.Common.Log;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Validation
{
    public class OperationValidationRepository : IOperationValidationRepository
    {
        private readonly INoSQLTableStorage<OperationValidationEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _index;

        public OperationValidationRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory)
        {
            _storage = AzureTableStorage<OperationValidationEntity>.Create(connectionStringManager, "RiskControlOperationValidations", logFactory);
            _index = AzureTableStorage<AzureIndex>.Create(connectionStringManager, "RiskControlOperationValidations", logFactory);
        }

        public async Task AddAsync(OperationValidation validation)
        {
            var entity = new OperationValidationEntity(validation);
            await _storage.CreateIfNotExistsAsync(entity);

            if (validation.Risk.IsResolutionRequired)
                await _index.CreateIfNotExistsAsync(new AzureIndex(OperationValidationEntity.IndexPk(), entity.PartitionKey, entity));
        }

        public async Task SaveAsync(OperationValidation validation)
        {
            var entity = new OperationValidationEntity(validation);
            await _storage.InsertOrReplaceAsync(entity);

            if (validation.Resolution != OperationValidationResolution.Unconfirmed)
                await _index.DeleteIfExistAsync(OperationValidationEntity.IndexPk(), entity.PartitionKey);
        }

        public async Task<IReadOnlyList<OperationValidation>> GetResolutionRequiredAsync()
        {
            var indexes = await _index.GetDataAsync(OperationValidationEntity.IndexPk());

            var entities = await _storage.GetDataAsync(indexes);

            return entities.Select(x => x.ToOperationValidation()).ToList();
        }

        public async Task<OperationValidation> TryGetAsync(Guid operationId)
        {
            var entity = await _storage.GetDataAsync(
                OperationValidationEntity.Partition(operationId),
                OperationValidationEntity.Row());

            return entity?.ToOperationValidation();
        }
    }
}
