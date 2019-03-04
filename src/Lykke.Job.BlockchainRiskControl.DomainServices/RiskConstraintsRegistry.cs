using System.Collections.Generic;
using System.Linq;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Services;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    public sealed class RiskConstraintsRegistry : IRiskConstraintsRegistry
    {
        private readonly ICollection<IRiskConstraint> _globalContraints;
        private readonly IDictionary<OperationType, ICollection<IRiskConstraint>> _constraintsByOperationType;
        private readonly IDictionary<string, ICollection<IRiskConstraint>> _constraintsByBlockchainType;
        private readonly IDictionary<(string, string), ICollection<IRiskConstraint>> _constraintsByBlockchainTypeAndAssetId;
        private readonly IDictionary<(string, OperationType), ICollection<IRiskConstraint>> _constraintsByBlockchainTypeAndOperationType;
        private readonly IDictionary<(string, string, OperationType), ICollection<IRiskConstraint>> _constraintsByBlockchainTypeAssetAndOperationType;

        public RiskConstraintsRegistry()
        {
            _globalContraints = new List<IRiskConstraint>();
            _constraintsByOperationType = new Dictionary<OperationType, ICollection<IRiskConstraint>>();
            _constraintsByBlockchainType = new Dictionary<string, ICollection<IRiskConstraint>>();
            _constraintsByBlockchainTypeAndAssetId = new Dictionary<(string, string), ICollection<IRiskConstraint>>();
            _constraintsByBlockchainTypeAndOperationType = new Dictionary<(string, OperationType), ICollection<IRiskConstraint>>();
            _constraintsByBlockchainTypeAssetAndOperationType = new Dictionary<(string, string, OperationType), ICollection<IRiskConstraint>>();
        }

        public void Add(IRiskConstraint constraint)
        {
            _globalContraints.Add(constraint);
        }

        public void Add(OperationType operationType, IRiskConstraint constraint)
        {
            _constraintsByOperationType.AddCollectionItem(operationType, constraint);
        }

        public void Add(string blockchainType, IRiskConstraint constraint)
        {
            _constraintsByBlockchainType.AddCollectionItem(blockchainType, constraint);
        }

        public void Add(string blockchainType, string blockchainAssetId, IRiskConstraint constraint)
        {
            _constraintsByBlockchainTypeAndAssetId.AddCollectionItem((blockchainType, blockchainAssetId), constraint);
        }

        public void Add(string blockchainType, OperationType operationType, IRiskConstraint constraint)
        {
            _constraintsByBlockchainTypeAndOperationType.AddCollectionItem((blockchainType, operationType), constraint);
        }

        public void Add(string blockchainType, string blockchainAssetId, OperationType operationType, IRiskConstraint constraint)
        {
            _constraintsByBlockchainTypeAssetAndOperationType.AddCollectionItem((blockchainType, blockchainAssetId, operationType), constraint);
        }

        public IEnumerable<(string, string, OperationType?, IRiskConstraint)> GetConstraints(string blockchainType, string blockchainAssetId, OperationType operationType)
        {
            var global = _globalContraints
                .Select(rc => (default(string), default(string), default(OperationType?), rc));

            var byOp =_constraintsByOperationType
                .GetCollectionItems(operationType)
                .Select(rc => (default(string), default(string), (OperationType?)operationType, rc));

            var byBcn = _constraintsByBlockchainType
                .GetCollectionItems(blockchainType)
                .Select(rc => (blockchainType, default(string), default(OperationType?), rc));

            var byBcnAndAsset = _constraintsByBlockchainTypeAndAssetId
                .GetCollectionItems((blockchainType, blockchainAssetId))
                .Select(rc => (blockchainType, blockchainAssetId, default(OperationType?), rc));

            var byBcnAndOp = _constraintsByBlockchainTypeAndOperationType
                .GetCollectionItems((blockchainType, operationType))
                .Select(rc => (blockchainType, default(string), (OperationType?)operationType, rc));

            var byBcnAndAssetAndOp = _constraintsByBlockchainTypeAssetAndOperationType
                .GetCollectionItems((blockchainType, blockchainAssetId, operationType))
                .Select(rc => (blockchainType, blockchainAssetId, (OperationType?)operationType, rc));

            return global
                .Concat(byOp)
                .Concat(byBcn)
                .Concat(byBcnAndAsset)
                .Concat(byBcnAndOp)
                .Concat(byBcnAndAssetAndOp)
                .ToList();
        }
    }
}
