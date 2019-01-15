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

        public IEnumerable<IRiskConstraint> GetConstraints(string blockchainType, string blockchainAssetId, OperationType operationType)
        {
            return _globalContraints
                .Concat(_constraintsByOperationType.GetCollectionItems(operationType))
                .Concat(_constraintsByBlockchainType.GetCollectionItems(blockchainType))
                .Concat(_constraintsByBlockchainTypeAndAssetId.GetCollectionItems((blockchainType, blockchainAssetId)))
                .Concat(_constraintsByBlockchainTypeAndOperationType.GetCollectionItems((blockchainType, operationType)))
                .Concat(_constraintsByBlockchainTypeAssetAndOperationType.GetCollectionItems((blockchainType, blockchainAssetId, operationType)));
        }
    }
}
