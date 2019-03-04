using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain.Services
{
    public interface IOperationValidationService
    {
        Task<OperationValidation> ValidateAsync(Operation operation);
        Task<OperationValidationResolution> WaitForResolutionAsync(Guid operationId);
    }
}
