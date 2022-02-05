using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    public interface ICamParameters : IEnumerable<ICamParameter>
    {
        ICamParameter this[int index] { get; }
        ICamParameter this[string key] { get; }
        IEnumerable<string> ParameterNames { get; }

        bool ContainParameter(string paramName);
    }
}
