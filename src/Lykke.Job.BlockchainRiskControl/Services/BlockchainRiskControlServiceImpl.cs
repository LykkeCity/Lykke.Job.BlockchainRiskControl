using System;
using System.Threading.Tasks;
using Common.Log;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainRiskControl.Contract;
using Lykke.Job.BlockchainRiskControl.Contract.Events;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.Service.BlockchainRiskControl;

namespace Lykke.Job.BlockchainRiskControl.Services
{
    public class BlockchainRiskControlServiceImpl : BlockchainRiskControlService.BlockchainRiskControlServiceBase
    {
        private readonly IOperationValidationRepository _validationRepository;
        private readonly ICqrsEngine _cqrsEngine;
        private readonly ILog _log;

        public BlockchainRiskControlServiceImpl(
            IOperationValidationRepository validationRepository,
            ICqrsEngine cqrsEngine,
            ILogFactory logFactory
            )
        {
            _log = logFactory.CreateLog(this);
            _validationRepository = validationRepository;
            _cqrsEngine = cqrsEngine;
        }

        public override async Task<OperationsResponse> GetOperations(Empty request, ServerCallContext context)
        {
            var operations = await _validationRepository.GetResolutionRequiredAsync();
            var result = new OperationsResponse();

            foreach (var operation in operations)
            {
                var riskOperation = new RiskOperation
                {
                    Id = operation.Operation.Id.ToString(),
                    Risk = new Risk
                    {
                        Violations = operation.Risk.Violations,
                        Level = operation.Risk.Level.ToString(),
                        IsReliable = operation.Risk.IsReliable,
                        IsResolutionRequired = operation.Risk.IsResolutionRequired,
                    }
                };

                result.Operations.Add(riskOperation);
            }

            return result;
        }

        public override async Task<Empty> AcceptOperation(OperationIdRequest request, ServerCallContext context)
        {
            var result = new Empty();

            if (string.IsNullOrEmpty(request.Id))
                return result;

            if (!Guid.TryParse(request.Id, out var operationId))
                return result;

            var operation = await _validationRepository.TryGetAsync(operationId);

            if (operation == null)
            {
                _log.Warning("Operation not found", context: request.Id);
                return result;
            }

            operation.Resolve(OperationValidationResolution.Accepted);
            await _validationRepository.SaveAsync(operation);

            _cqrsEngine.PublishEvent(new OperationAcceptedEvent {OperationId = operationId},
                BlockchainRiskControlBoundedContext.Name);

            return result;
        }

        public override async Task<Empty> RejectOperation(OperationIdRequest request, ServerCallContext context)
        {
            var result = new Empty();

            if (string.IsNullOrEmpty(request.Id))
                return result;

            if (!Guid.TryParse(request.Id, out var operationId))
                return result;

            var operation = await _validationRepository.TryGetAsync(operationId);

            if (operation == null)
            {
                _log.Warning("Operation not found", context: request.Id);
                return result;
            }

            operation.Resolve(OperationValidationResolution.Rejected);
            await _validationRepository.SaveAsync(operation);

            _cqrsEngine.PublishEvent(new OperationRejectedEvent {OperationId = operationId},
                BlockchainRiskControlBoundedContext.Name);

            return result;
        }
    }
}
