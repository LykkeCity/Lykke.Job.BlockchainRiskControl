using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Job.BlockchainRiskControl.Settings.JobSettings
{
    [UsedImplicitly]
    public class BlockchainRiskControlJobSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public DbSettings Db { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public CqrsSettings Cqrs { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IList<RiskConstraintsGroupSettings> ConstraintsGroups { get; set; }
    }
}
