using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Job.BlockchainRiskControl.Settings.JobSettings
{
    [UsedImplicitly]
    public class RiskConstraintsGroupSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Blockchain { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Asset { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public Domain.OperationType Operation { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IList<RiskConstraintSettings> Constraints { get; }
    }
}
