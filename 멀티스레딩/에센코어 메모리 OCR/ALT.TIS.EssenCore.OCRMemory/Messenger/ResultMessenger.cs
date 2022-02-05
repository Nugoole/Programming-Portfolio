using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.TIS.EssenCore.OCRMemory.Messenger
{
    public class ResultMessenger
    {
        public int Index { get; set; }
        public bool PassFail { get; set; }
        public string Detail { get; set; }

        public ResultMessenger(int index, bool passFail, string detail = "")
        {
            Index = index;
            PassFail = passFail;
            Detail = detail;
        }
    }
}
