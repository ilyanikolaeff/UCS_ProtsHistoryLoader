using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UCS_ProtsHistoryLoader
{
    public class ProtEntryConfig
    {
        public string Tag { get; set; }
        public string ImitTag { get; set; }
        public object TargetValue { get; set; }
    }
}
