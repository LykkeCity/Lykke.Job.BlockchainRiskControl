using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Validation
{
    public class OperationValidationRepository : IOperationValidationRepository
    {
        private readonly INoSQLTableStorage<OperationValidationEntity> _storage;

        public OperationValidationRepository(IReloadingManager<string> connectionStringManager, ILogFactory logFactory)
        {
            _storage = AzureTableStorage<OperationValidationEntity>.Create(connectionStringManager, "OperationValidations", logFactory);
        }

        public async Task AddAsync(OperationValidation validation) =>
            await _storage.CreateIfNotExistsAsync(new OperationValidationEntity(validation));

        public async Task SaveAsync(OperationValidation validation) =>
            await _storage.ReplaceAsync(new OperationValidationEntity(validation));

        public async Task<OperationValidation> TryGetAsync(Guid operationId)
        {
            var entity = await _storage.GetDataAsync(
                OperationValidationEntity.Partition(operationId),
                OperationValidationEntity.Row());

            return entity?.ToOperationValidation();
        }
    }
}