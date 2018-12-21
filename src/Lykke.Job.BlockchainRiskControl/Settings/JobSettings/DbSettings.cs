﻿using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainRiskControl.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
