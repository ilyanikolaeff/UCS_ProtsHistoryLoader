using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_ProtsHistoryLoader
{
    class Protection
    {
        /// <summary>
        /// Имя тега
        /// </summary>
        public string TagName { get; set; }
        
        /// <summary>
        /// Описание (взятое по OPC DA из свойства Description)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Время сработки
        /// </summary>
        public DateTime ActivateTime { get; set; }
        public DateTime DeactivateTime { get; set; }
        public TimeSpan Duration { get => DeactivateTime.Subtract(ActivateTime); }
        /// <summary>
        /// Входа защиты
        /// </summary>
        public List<ProtectionEntry> ProtectionEntries { get; set; }
        public Protection()
        {
            ProtectionEntries = new List<ProtectionEntry>();
        }

        public override string ToString()
        {
            return $"{TagName}\t{ActivateTime}\t{DeactivateTime}\t{Duration}\t{Description}";
        }
    }
}
