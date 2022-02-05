using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Core
{
    [Serializable]
    public abstract class SerializableObjectBase : ISerializable, IDeserializationCallback
    {
        protected SerializableObjectBase()
        {

        }

        protected SerializableObjectBase(SerializationInfo info, StreamingContext context)
        {
            var fieldInfos = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var fieldInfo in fieldInfos)
            {
                if (!fieldInfo.IsNotSerialized)
                    fieldInfo.SetValue(this, info.GetValue(fieldInfo.Name, fieldInfo.FieldType));
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("SerializationInfo is Null");

            var fieldInfos = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (var fieldInfo in fieldInfos)
            {
                var value = fieldInfo.GetValue(this);

                if (!fieldInfo.IsNotSerialized)
                    info.AddValue(fieldInfo.Name, value, fieldInfo.FieldType);
            }
        }

        public virtual void OnDeserialization(object sender)
        {

        }
    }
}
