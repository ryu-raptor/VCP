using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VCP.API
{
    /// <summary>
    /// Registor of API and API name
    /// </summary>
    public static class APINameRegister
    {
        static Dictionary<Type, string> nameMap = new Dictionary<Type, string>();

        public static void Regist<T>(string name)
        {
            nameMap.Add(typeof(T), name);
        }

        public static string Query<T>()
            where T : APIBase, new()
        {
            string result;
            if (nameMap.TryGetValue(typeof(T), out result)) {
                return result;
            }
            // Initialize Type then retry
            Regist<T>((new T()).Type);
            // no recursion because of performance
            return nameMap[typeof(T)];
        }

        public static string Query(Type type)
        {
            string result;
            if (nameMap.TryGetValue(type, out result)) {
                return result;
            }
            return null;
        }
    }
}