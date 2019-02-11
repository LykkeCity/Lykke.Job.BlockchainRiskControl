using System.Collections.Generic;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainRiskControl.Settings.JobSettings
{
    [UsedImplicitly]
    public class RiskConstraintsGroupSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        [Optional]
        public string Blockchain { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        [Optional]
        public string Asset { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        [Optional]
        public Domain.OperationType? Operation { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public IList<RiskConstraintSettings> Constraints { get; set; }
    }
}
