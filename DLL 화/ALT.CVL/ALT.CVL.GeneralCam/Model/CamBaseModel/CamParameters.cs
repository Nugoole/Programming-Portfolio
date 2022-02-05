using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.CamBaseModel
{
    internal class CamParameters : Dictionary<string, ICamParameter>, ICamParameters
    {
        public ICamParameter this[int index]
        {
            get
            {
                try
                {
                    return Values.ElementAt(index);
                }
                catch (Exception e)
                {
                    if (e is ArgumentNullException || e is ArgumentOutOfRangeException)
                        throw e;

                    return null;
                }
                
            }
        }

        public IEnumerable<string> ParameterNames => Keys;

        public bool ContainParameter(string paramName)
        {
            return ContainsKey(paramName);
        }

        IEnumerator<ICamParameter> IEnumerable<ICamParameter>.GetEnumerator()
        {
            return new CamParametersEnumerator(this);
        }
    }

    class CamParametersEnumerator : IEnumerator<ICamParameter>
    {
        CamParameters origin;
        int position = -1;
        public ICamParameter Current
        {
            get
            {
                try
                {
                    return origin.Values.ElementAt(position);
                }
                catch (Exception e)
                {
                    if (e is ArgumentOutOfRangeException || e is ArgumentNullException)
                        throw e;

                    return null;
                }
            }
        }
        object IEnumerator.Current => Current;

        public CamParametersEnumerator(CamParameters original)
        {
            origin = original;
        }

        public void Dispose()
        {
            position = -1;
        }

        public bool MoveNext()
        {
            position++;
            return position < origin.Count;
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
