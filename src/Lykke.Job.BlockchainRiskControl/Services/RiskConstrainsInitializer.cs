using System.Collections.Generic;
using System.Linq;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration;
using Lykke.Job.BlockchainRiskControl.Domain.Services;
using Lykke.Job.BlockchainRiskControl.Settings.JobSettings;

namespace Lykke.Job.BlockchainRiskControl.Services
{
    public class RiskConstrainsInitializer
    {
        private readonly ILog _log;

        private readonly IRiskConstraintsRegistryConfigurator _riskConstraintsRegistryConfigurator;
        private readonly IList<RiskConstraintsGroupSettings> _riskConstraintsGroupsSettings;

        public RiskConstrainsInitializer(
            ILogFactory logFactory,
            IRiskConstraintsRegistryConfigurator riskConstraintsRegistryConfigurator,
            IList<RiskConstraintsGroupSettings> riskConstraintsGroupsSettings)
        {
            _log = logFactory.CreateLog(this);

            _riskConstraintsRegistryConfigurator = riskConstraintsRegistryConfigurator;
            _riskConstraintsGroupsSettings = riskConstraintsGroupsSettings;
        }

        public void Initialize()
        {
            for (var i = 0; i < _riskConstraintsGroupsSettings.Count; ++i)
            {
                var groupSettings = _riskConstraintsGroupsSettings[i];

                _log.Info($"Configuring risk constraints group [{i}]...", new
                {
                    groupSettings.Operation,
                    groupSettings.Blockchain,
                    groupSettings.Asset
                });

                var groupConfiguration = new RiskConstraintsGroupConfiguration
                (
                    operationType: groupSettings.Operation,
                    blockchainType: groupSettings.Blockchain,
                    blockchainAssetId: groupSettings.Asset,
                    constraints: groupSettings.Constraints
                        .Select(s => new RiskConstraintConfiguration(s.Name, s.Parameters))
                        .ToArray()
                );

                _riskConstraintsRegistryConfigurator.Configure(groupConfiguration);
            }
        }
    }
}
