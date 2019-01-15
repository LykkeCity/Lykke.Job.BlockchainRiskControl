using Lykke.Job.BlockchainRiskControl.Domain;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    internal static class RiskContraintExtensions
    {
        public static string GetConstraintName(this IRiskConstraint constraint)
        {
            return constraint.GetType().GetDisplayName();
        }
    }
}
