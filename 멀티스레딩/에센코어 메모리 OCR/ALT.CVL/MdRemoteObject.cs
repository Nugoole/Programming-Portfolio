using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{
    public class MdRemoteObject : MarshalByRefObject
    {
        public MdRemoteObject()
        {

        }

        public string ReplyMessage(string massage)
        {
            return "Server alive";
        }
    }
}
