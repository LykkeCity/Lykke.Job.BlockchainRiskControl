using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Job.BlockchainRiskControl.Settings.JobSettings
{
    [UsedImplicitly]
    public class RiskConstraintSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string Name { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public Dictionary<string, string> Parameters { get;set; }
    }
}