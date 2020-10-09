using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Grpc.Core;
using Lykke.Common.Log;
using Lykke.Sdk;

namespace Lykke.Job.BlockchainRiskControl.Services
{
    public class ShutdownManager : IShutdownManager
    {
        private readonly ILog _log;
        private readonly IEnumerable<IStopable> _items;
        private readonly Server _grpcServer;

        public ShutdownManager(ILogFactory logFactory, IEnumerable<IStopable> items, Server grpcServer)
        {
            _log = logFactory.CreateLog(this);
            _items = items;
            _grpcServer = grpcServer;
        }

        public async Task StopAsync()
        {
            foreach (var item in _items)
            {
                try
                {
                    item.Stop();
                }
                catch (Exception ex)
                {
                    _log.Warning($"Unable to stop {item.GetType().Name}", ex);
                }
            }

            await _grpcServer.ShutdownAsync();
        }
    }
}
