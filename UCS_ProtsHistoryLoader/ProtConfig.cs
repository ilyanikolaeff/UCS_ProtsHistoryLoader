using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UCS_ProtsHistoryLoader
{
    [Serializable]
    public class ProtConfig
    {
        [XmlAttribute]
        public string Tag { get; set; }
        [XmlAttribute]
        public int MinDuration { get; set; }
        public List<ProtEntryConfig> ProtEntriesConfig { get; set; }
        public ProtConfig()
        {
            ProtEntriesConfig = new List<ProtEntryConfig>();
        }
    }
}
