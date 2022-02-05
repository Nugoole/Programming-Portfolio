using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    public interface ICamParameter
    {
        bool IsReadOnly { get; }

        string Name { get; }
        string Value { get; set; }
        Type Type { get; }
    }
}
