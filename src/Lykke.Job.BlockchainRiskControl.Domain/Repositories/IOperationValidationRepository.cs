using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain.Repositories
{
    public interface IOperationValidationRepository
    {
        Task<OperationValidation> TryGetAsync(Guid operationId);
        Task AddAsync(OperationValidation validation);
        Task SaveAsync(OperationValidation validation);
        Task<IReadOnlyList<OperationValidation>> GetResolutionRequiredAsync();
    }
}
