using System.Collections.Generic;

namespace Lykke.Job.BlockchainRiskControl.Domain.Services
{
    public interface IRiskConstraintsRegistry
    {
        void Add(IRiskConstraint constraint);
        void Add(OperationType operationType, IRiskConstraint constraint);
        void Add(string blockchainType, IRiskConstraint constraint);
        void Add(string blockchainType, string blockchainAssetId, IRiskConstraint constraint);
        void Add(string blockchainType, OperationType operationType, IRiskConstraint constraint);
        void Add(string blockchainType, string blockchainAssetId, OperationType operationType, IRiskConstraint constraint);
        IEnumerable<IRiskConstraint> GetConstraints(string blockchainType, string blockchainAssetId, OperationType operationType);
    }
}
