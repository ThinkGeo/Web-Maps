using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    /// <summary>
    /// Helper class to serialize or deserialize a generic object to/from JSON.
    /// </summary>
    public class JsonSerializer
    {
        /// <summary>
        /// Serializes an object to JSON 
        /// </summary>
        /// <typeparam name="T">Type of object to serialize</typeparam>
        /// <param name="obj">Instance of object to serialize</param>
        /// <returns>JSON string</returns>
        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }

        /// <summary>
        /// Deserialize an object from JSON
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize</typeparam>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>deserialized object</returns>
        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            ms.Dispose();
            return obj;
        }
    }
}