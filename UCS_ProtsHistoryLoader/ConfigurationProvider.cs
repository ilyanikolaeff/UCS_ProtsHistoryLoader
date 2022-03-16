using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UCS_ProtsHistoryLoader
{
    class ConfigurationProvider
    {
        private static ConfigurationProvider _instance;
        public static ConfigurationProvider GetInstance()
        {
            if (_instance == null)
                _instance = new ConfigurationProvider();
            return _instance;
        }
        public bool IsInitialized = false;
        public List<ProtConfig> ProtConfigs { get; private set; }

        private ConfigurationProvider()
        {
            ProtConfigs = new List<ProtConfig>();
        }

        public void Load(string xmlFileName)
        {
            var xmlSerializer = new XmlSerializer(ProtConfigs.GetType());
            using (var reader = new StreamReader(xmlFileName))
            {
                ProtConfigs = (List<ProtConfig>)xmlSerializer.Deserialize(reader);
            }
            IsInitialized = true;
        }
    }
}
