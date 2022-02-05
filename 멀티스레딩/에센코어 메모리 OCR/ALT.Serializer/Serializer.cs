using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ALT.Serialize
{
    public static class Serializer
    {
        public static void Save<T>(T saveObject, string filePath) where T : ISerializable
        {
            BinaryFormatter formatter = new BinaryFormatter();
            StreamingContext context = new StreamingContext(StreamingContextStates.Persistence | StreamingContextStates.File);
            formatter.Context = context;

            if (File.Exists(filePath))
                File.Delete(filePath);

            using (FileStream stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                formatter.Serialize(stream, saveObject);
            }
        }

        public static object Load(string filePath)
        {
            object objectToReturn;

            BinaryFormatter formatter = new BinaryFormatter();
            StreamingContext context = new StreamingContext(StreamingContextStates.Persistence | StreamingContextStates.File);
            formatter.Context = context;
            
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                objectToReturn = formatter.Deserialize(stream);
            }

            return objectToReturn;
        }
    }
}
