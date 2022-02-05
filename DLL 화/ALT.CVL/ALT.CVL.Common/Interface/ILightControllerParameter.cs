using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{
    public interface ILightControllerParameter<T>
    {
        string Name { get; }
        bool UpdateImmediately { get; set; }
        T Value { get; set; }


        void Set();
        void Refresh();

        event EventHandler<T> OnValueChanged;
    }
}
