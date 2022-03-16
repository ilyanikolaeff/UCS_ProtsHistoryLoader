using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_ProtsHistoryLoader
{
    public abstract class ProtectionEntry
    {
        public string TagName { get; set; }
        public string Description { get; set; }

    }

    public class ValveProtectionEntry : ProtectionEntry
    {
        public object Value;
        public DateTime Timestamp;

        public override string ToString()
        {
            return $"\t{TagName}\t{Timestamp}\t{Value}\t{Description}";
        }
    }
}
