﻿using JetBrains.Annotations;
using Lykke.Job.BlockchainRiskControl.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Lykke.Logs.Loggers.LykkeSlack;
using Lykke.Sdk;

namespace Lykke.Job.BlockchainRiskControl
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "BlockchainRiskControlJob API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "BlockchainRiskControlJobLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.BlockchainRiskControlJob.Db.LogsConnString;

                    logs.Extended = extendedLogs =>
                    {
                        extendedLogs.AddAdditionalSlackChannel("CommonBlockChainIntegration", channelOptions =>
                        {
                            channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
                            channelOptions.SpamGuard.DisableGuarding();
                            channelOptions.IncludeHealthNotifications();
                        });
                        
                        extendedLogs.AddAdditionalSlackChannel("CommonBlockChainIntegrationImportantMessages", channelOptions =>
                        {
                            channelOptions.MinLogLevel = Microsoft.Extensions.Logging.LogLevel.Warning;
                            channelOptions.SpamGuard.DisableGuarding();
                            channelOptions.IncludeHealthNotifications();
                        });
                    };

                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
