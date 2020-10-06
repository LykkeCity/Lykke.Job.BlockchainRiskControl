using Autofac;
using Grpc.Core;
using JetBrains.Annotations;
using Lykke.Service.BlockchainRiskControl;

namespace Lykke.Job.BlockchainRiskControl.Contract
{
    [PublicAPI]
    public static class AutofacExtensions
    {
        /// <summary>
        ///     Registers <see cref="BlockchainRiskControlService.BlockchainRiskControlServiceClient" /> in Autofac container.
        /// </summary>
        /// <param name="builder">Autofac container builder.</param>
        /// <param name="serviceUrl">Service grpc url.</param>
        public static void RegisterBlockchainRiskControlClient(this ContainerBuilder builder, string serviceUrl)
        {
            builder.Register(ctx =>
            {
                var channel = new Channel(serviceUrl, SslCredentials.Insecure);
                return new BlockchainRiskControlService.BlockchainRiskControlServiceClient(channel);
            }).As<BlockchainRiskControlService.BlockchainRiskControlServiceClient>();
        }
    }
}
