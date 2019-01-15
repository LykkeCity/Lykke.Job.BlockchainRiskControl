using System;
using System.ComponentModel;
using System.Reflection;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    internal static class TypeExtensions
    {
        public static string GetDisplayName(this Type type)
        {
            return type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
        }
    }
}
