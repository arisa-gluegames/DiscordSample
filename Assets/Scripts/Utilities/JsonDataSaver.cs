using UnityEngine;
using System.IO;
using System.Reflection;
using System;

namespace GlueGames.Utilities
{
    public class JsonDataSaver<T>
    {
        private static readonly string SavePath = Application.persistentDataPath;
        private static readonly string FileExtension = ".json";

        public static void Save(string fileName, T data)
        {
            string filePath = Path.Combine(SavePath, fileName + FileExtension);

            using StreamWriter writer = new(filePath);
            string jsonData = JsonUtility.ToJson(data);
            writer.Write(jsonData);
            writer.Close();
        }

        public static bool FileExists(string fileName)
        {
            string filePath = Path.Combine(SavePath, fileName + FileExtension);
            return File.Exists(filePath);
        }

        // Load data from a JSON file
        public static T Load(string fileName, T defaultValue)
        {
            string filePath = Path.Combine(SavePath, fileName + FileExtension);

            if (File.Exists(filePath))
            {
                using StreamReader reader = new(filePath);
                string jsonData = reader.ReadToEnd();
                reader.Close();
                return JsonUtility.FromJson<T>(jsonData);
            }
            else
            {
                Debug.LogWarning($"File not found: {filePath}");
                return defaultValue;
            }
        }

        public static bool IsDataComplete(T data)
        {
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();
            FieldInfo[] fields = type.GetFields();

            foreach (var property in properties)
            {
                if (property.GetValue(data) == null)
                {
                    return false;
                }
            }
            foreach (var field in fields)
            {
                if (field.GetValue(data) == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
