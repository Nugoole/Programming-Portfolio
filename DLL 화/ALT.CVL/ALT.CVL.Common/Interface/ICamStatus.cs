using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    public interface ICamStatus
    {
        bool IsConnected { get; }
        bool IsLive { get; set; }
    }
}
