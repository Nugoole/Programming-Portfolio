using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Model
{
    public class Parameter : ICamParameter
    {
        private string name;
        private bool isReadOnly;
        private Type valueType;
        private Func<string, string> getter;
        private IEnumerable<string> availableValues;

        private event EventHandler<string> OnValueSet;
        public string Name => name;
        public string Value
        {
            get
            {
                if (getter != null)
                {
                    try
                    {
                        return getter(Name);
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }

                return string.Empty;
            }
            set
            {
                OnValueSet?.Invoke(this, value);
            }
        }
        public IEnumerable<string> AvailableValues => availableValues;
        public bool IsReadOnly => isReadOnly;

        public Type Type => valueType;

        public Parameter(string name, string value, bool isReadOnly, Type valueType = null,Func<string,string> commonGetter = null, EventHandler<string> commonSetter = null, IEnumerable<string> valueRange = null)
        {
            this.name = name;
            this.isReadOnly = isReadOnly;
            this.valueType = valueType;
            this.availableValues = valueRange;

            if(commonSetter != null)
                OnValueSet = commonSetter;
            if (commonGetter != null)
                getter = commonGetter;

        }
    }
}
