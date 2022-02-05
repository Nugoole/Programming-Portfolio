using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{
    internal class LightControllerParameter<T> : ILightControllerParameter<T>, INotifyPropertyChanged
    {
        private string name = default;
        private Func<int, T> getter;
        private Action<int, T> setter;
        private int index = -1;
        private T value = default;


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<T> OnValueChanged;

        internal void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        public string Name => name;
        public bool UpdateImmediately { get; set; } = true;
        public T Value
        {
            get
            {
                if (UpdateImmediately)
                    return getter(index);
                else
                    return value;
            }
            set
            {
                if (UpdateImmediately)
                {
                    setter(index, value);
                    OnValueChanged?.Invoke(this, Value);
                }
                else
                    this.value = value;

                RaisePropertyChanged();
            }
        }

        internal LightControllerParameter(string paramName,int idx, Func<int, T> getter, Action<int, T> setter)
        {
            name = paramName;
            index = idx;
            this.getter = getter;
            this.setter = setter;

            this.value = getter(index);
        }


        public void Set()
        {
            setter(index, this.value);
            RaisePropertyChanged(nameof(Value));

            OnValueChanged?.Invoke(this, Value);
        }
        public void Refresh()
        {
            RaisePropertyChanged(nameof(Value));
        }
    }
}
