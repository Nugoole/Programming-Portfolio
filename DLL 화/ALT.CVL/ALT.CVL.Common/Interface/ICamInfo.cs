using ALT.CVL.Common.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    public interface ICamInfo
    {
        string CamName { get; }
        string SerialNumber { get; }
        InterfaceType InterfaceType { get; }
    }
}
