using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WaitAndEat.Utils
{
    /// <summary>
    /// Provides JSON serialization/deserialization functionality.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// Serializes object to JSON string representation
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <returns>JSON representation of the object. Returns 'null' string for null passed as argument</returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Parses json string to object instance
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="json">json string representation of the object</param>
        /// <returns>Deserialized object instance</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static List<T> DeserializeList<T>(string json)
        {
            dynamic data = JsonConvert.DeserializeObject<dynamic>(json);
            List<T> list = new List<T>();
            foreach (var itemDynamic in data)
            {
                list.Add(JsonConvert.DeserializeObject<T>(((JProperty)itemDynamic).Value.ToString()));
            }
            return list;
        }
    }
}