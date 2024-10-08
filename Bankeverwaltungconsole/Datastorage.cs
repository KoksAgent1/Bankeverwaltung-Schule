﻿using Newtonsoft.Json;
using System.IO;

namespace Bankeverwaltungconsole
{
    internal class Datastorage
    {
        private static string dataFilePath = "bankdata.json";
        // Methode zum Speichern der Bankdaten in einer JSON-Datei
        public static void SaveData(Bank bank)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                //PreserveReferencesHandling = PreserveReferencesHandling.All,
                Formatting = Formatting.Indented
            };
            string jsonData = JsonConvert.SerializeObject(bank, Formatting.Indented, settings);
            File.WriteAllText(dataFilePath, jsonData);
        }

        // Methode zum Laden der Bankdaten aus einer JSON-Datei
        public static Bank LoadData()
        {
            if (!File.Exists(dataFilePath))
            {
                return null; // Hier könnte auch eine neue Bankinstanz initialisiert werden
            }
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                //PreserveReferencesHandling = PreserveReferencesHandling.All,
            };
            string jsonData = File.ReadAllText(dataFilePath);
            return JsonConvert.DeserializeObject<Bank>(jsonData, settings);
        }
    }
}
