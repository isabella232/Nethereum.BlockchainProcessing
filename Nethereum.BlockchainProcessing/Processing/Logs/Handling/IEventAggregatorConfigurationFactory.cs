﻿using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface IEventAggregatorConfigurationFactory
    {
        Task<EventAggregatorConfiguration> GetEventAggregationConfiguration(long eventSubscriptionId);
    }
}