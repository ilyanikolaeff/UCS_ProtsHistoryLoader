using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_ProtsHistoryLoader
{
    class Converter
    {
        public IEnumerable<ProtConfig> ConvertFromTxt(string fileName)
        {
            var fileLines = File.ReadAllLines(fileName);
            ProtConfig currentProtection = null;
            foreach (var line in fileLines)
            {
                var lineParts = line.Split('\t');
                if (lineParts.All(p => string.IsNullOrEmpty(p)))
                    continue;
                if (!string.IsNullOrEmpty(lineParts[0]))
                {
                    currentProtection = new ProtConfig
                    {
                        Tag = lineParts[0]
                    };
                    yield return currentProtection;
                }
                else
                {
                    currentProtection.ProtEntriesConfig.Add(new ProtEntryConfig() { Tag = lineParts[1], ImitTag = lineParts[2], TargetValue = int.Parse(lineParts[3]) });
                }
            }
        }
    }
}
