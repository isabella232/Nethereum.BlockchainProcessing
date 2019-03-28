﻿using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Matching;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    /// <summary>
    /// a dynamically loaded processor
    /// Designed to be instantiated from DB configuration data
    /// </summary>

    public class LogProcessor : ILogProcessor
    {
        private readonly IEventMatcher _matcher;
        private readonly IEventHandler _handler;

        public LogProcessor(IEventMatcher matcher, IEventHandler handler)
        {
            _matcher = matcher ?? throw new System.ArgumentNullException(nameof(matcher));
            _handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
        }

        public bool IsLogForEvent(FilterLog log)
        {
            return _matcher.IsMatch(log);
        }

        public Task ProcessLogsAsync(params FilterLog[] eventLogs)
        {
            return _handler.HandleAsync(_matcher.Abi, eventLogs);
        }
    }
}