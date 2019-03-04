using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain.Repositories
{
    // TODO: SHould be Azure table storage
    public interface IOperationValidationRepository
    {
        Task<OperationValidation> TryGetAsync(Guid operationId);
        Task AddAsync(OperationValidation validation);
        Task SaveAsync(OperationValidation validation);
    }
}
