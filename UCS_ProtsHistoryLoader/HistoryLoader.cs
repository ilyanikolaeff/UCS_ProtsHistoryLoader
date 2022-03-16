using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCWrapper.HistoricalDataAccess;

namespace UCS_ProtsHistoryLoader
{
    class HistoryLoader
    {
        private readonly string _ipAddress;
        private readonly string _serverName;
        public HistoryLoader(string ipAddress, string serverName)
        {
            _ipAddress = ipAddress;
            _serverName = serverName;
        }

        private OpcHdaClient _opcHdaClient;

        public void Init()
        {
            _opcHdaClient = new OpcHdaClient(new OPCWrapper.ConnectionSettings(_ipAddress, _serverName));
            _opcHdaClient.Connect();
        }

        public event EventHandler<NotifyEventArgs> ProgressChanged;

        // maybe await 
        public async Task<IEnumerable<Protection>> Load(ProtConfig protConfig, DateTime startTime, DateTime endTime)
        {
            var resultsCollection = new List<Protection>();
            try
            {
                ProgressChanged?.Invoke(this, new NotifyEventArgs($"Обработка {protConfig.Tag}"));
                var prots = await CheckProtection(protConfig, startTime, endTime);
                resultsCollection.AddRange(prots);
            }
            catch (Exception ex)
            {
                ProgressChanged?.Invoke(this, new NotifyEventArgs($"Ошибка обработки {protConfig.Tag}.\n{ex}"));
            }
            return resultsCollection;
        }

        private Task<List<Protection>> CheckProtection(ProtConfig protConfig, DateTime startTime, DateTime endTime)
        {
            var results = _opcHdaClient.ReadRaw(startTime, endTime, new List<string>() { protConfig.Tag })[0].FilterResults(FilterType.GoodAndNotNull); // берем только хорошие
            var prots = new List<Protection>();
            // проходим по всем сработкам защиты
            var activateResults = results.Where(p => Convert.ToBoolean(p.Value));
            var deactivateResults = results.Where(p => !Convert.ToBoolean(p.Value));
            foreach (var result in activateResults)
            {
                var activateTimestamp = result.Timestamp; // время сработки
                // проверяем длительность работы защиты
                if (CheckDuration(deactivateResults, activateTimestamp, protConfig.MinDuration, out DateTime? deactivateTimestamp))
                {
                    // проверяем входа
                    if (CheckEntries(protConfig.ProtEntriesConfig, activateTimestamp, out List<ProtectionEntry> protectionEntries))
                    {
                        var currentProtection = new Protection
                        {
                            TagName = protConfig.Tag,
                            ActivateTime = activateTimestamp,
                            DeactivateTime = deactivateTimestamp ?? default,
                            ProtectionEntries = protectionEntries
                        };
                        prots.Add(currentProtection);
                    }
                }
            }
            return Task.FromResult(prots);
        }

        private bool CheckDuration(IEnumerable<OpcHdaResultItem> deactivateResults, DateTime activateTimestamp, int minDuration, out DateTime? deactivateTimestamp)
        {         
            if (minDuration <= 0)
            {
                deactivateTimestamp = activateTimestamp;
                return true;
            }

            var findedDeactivateTimestamp = deactivateResults.Where(p => p.Timestamp >= activateTimestamp)?.FindClosestResult(activateTimestamp)?.Timestamp;
            if (findedDeactivateTimestamp.HasValue)
            {
                deactivateTimestamp = findedDeactivateTimestamp.Value;
                return TimeSpan.FromTicks(findedDeactivateTimestamp.Value.Ticks - activateTimestamp.Ticks).TotalSeconds >= minDuration;
            }
            else
            {
                deactivateTimestamp = null;
                return false;
            }
        }

        private bool CheckEntries(List<ProtEntryConfig> protEntriesConfigs, DateTime timestamp, out List<ProtectionEntry> protectionEntries)
        {
            protectionEntries = new List<ProtectionEntry>();
            foreach (var protEntryConfig in protEntriesConfigs)
            {
                var tags = new List<string>
                {
                    protEntryConfig.Tag,
                    protEntryConfig.ImitTag
                };


                var readResults = _opcHdaClient.ReadRaw(timestamp.AddMinutes(-1), timestamp.AddMinutes(1), tags);
                var states = readResults[0].FilterResults(FilterType.GoodAndNotNull);
                var imits = readResults[1].FilterResults(FilterType.GoodAndNotNull);

                var state = states.FindClosestResult(timestamp);
                var isImit = imits.FindClosestResult(timestamp);

                if (Convert.ToDouble(state.Value) != Convert.ToDouble(protEntryConfig.TargetValue) || Convert.ToBoolean(isImit.Value))
                {
                    return false;
                }
                else
                {
                    protectionEntries.Add(new ValveProtectionEntry() { TagName = readResults[0].ItemName, Timestamp = state.Timestamp, Value = state.Value });
                }
            }
            return true;
        }
    }

    class NotifyEventArgs : EventArgs
    {
        public readonly DateTime Time;
        public readonly string Messsage;
        public NotifyEventArgs(string message)
        {
            Messsage = message;
            Time = DateTime.Now;
        }
    }
}
