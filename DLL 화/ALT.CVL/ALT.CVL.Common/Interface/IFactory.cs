using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    public interface IFactory<T,P>
    {
        T Create(P id);
    }

    public interface IFactory<T>
    {
        T Create();
    }
}
