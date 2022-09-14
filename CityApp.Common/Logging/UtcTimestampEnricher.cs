﻿using Serilog.Core;
using Serilog.Events;

namespace CityApp.Common.Logging
{
    /// <summary>
    /// Serilog enricher that convers the log event's timestamp to UTC.
    /// </summary>
    public class UtcTimestampEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory lepf)
        {
            logEvent.AddPropertyIfAbsent(lepf.CreateProperty("UtcTimestamp", logEvent.Timestamp.UtcDateTime));
        }
    }

}
