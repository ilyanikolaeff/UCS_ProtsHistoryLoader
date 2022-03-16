using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCS_ProtsHistoryLoader
{
    class ExportHelper
    {
        public static void Export(IEnumerable<Protection> protections)
        {
            var fileName = "SivkovKakSam.txt";
            using (var writer = new StreamWriter(fileName))
            {
                foreach (var prot in protections)
                {
                    writer.WriteLine(prot);
                    foreach (var protEntry in prot.ProtectionEntries)
                        writer.WriteLine(protEntry);
                }
            }
        }
    }
}
