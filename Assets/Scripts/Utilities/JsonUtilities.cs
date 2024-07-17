using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlueGames.Utilities
{
    public static class JsonUtilities
    {
        public static T GetData<T>(this JObject json, string key)
        {
            return json[key].ToObject<T>();
        }

        public static T GetData<T>(this JObject json, string key, T defaultValue)
        {
            if (!json.ContainsKey(key)) return defaultValue;
            return json[key].ToObject<T>();
        }

        public static bool TryGetData<T>(this JObject json, string key, out T value)
        {
            value = default;

            if (!json.ContainsKey(key)) return false;
            value = json[key].ToObject<T>();
            return true;
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}