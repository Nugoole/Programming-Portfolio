using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    public interface IConnectable<T_ObjectID>
    {
        void Connect(T_ObjectID id);

        void Disconnect();
    }

    public interface IConnectable
    {
        void Connect();

        void Disconnect();
    }
}
