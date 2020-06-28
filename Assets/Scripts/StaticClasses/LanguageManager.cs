using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public enum Language
{
    English = 0,
    German = 1,
    French = 2,
    Dutch = 3
}

public static class LanguageManager
{


    public static Language language = Language.English;

    private static LanguageTags languageTags;
    private static Dictionary<string, string> localisedEN;
    private static Dictionary<string, string> localisedDE;
    private static Dictionary<string, string> localisedFR;
    private static Dictionary<string, string> localisedNL;

    public static bool isInit;

    public static void Init()
    {
        Debug.Log("Init");
        // Load data
        LoadLanguagesTags();

        // assign dictionary foreach language
        localisedEN = GenerateDictionary((int)Language.English);
        localisedDE = GenerateDictionary((int)Language.German);
        localisedFR = GenerateDictionary((int)Language.French);
        localisedNL = GenerateDictionary((int)Language.Dutch);

        isInit = true;
    }

    public static string GetLocalisedValue(string key)
    {
        if (!isInit)
            Init();

        string value = key;

        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.German:
                localisedDE.TryGetValue(key, out value);
                break;
            case Language.French:
                localisedFR.TryGetValue(key, out value);
                break;
            case Language.Dutch:
                localisedNL.TryGetValue(key, out value);
                break;
        }

        return value;
    }

    private static void LoadLanguagesTags()
    {
        string fileName = GetFilePath("LanguageTags.json");
        //string _TestPath = "Assets/Resources/LanguageTags.json";

        using (StreamReader r = new StreamReader(fileName))
        {
            string json = r.ReadToEnd();
            Debug.Log(json);
            languageTags = JsonConvert.DeserializeObject<LanguageTags>(json);
            Debug.Log("LoadedLanguageTags");

        }
    }

    private static Dictionary<string, string> GenerateDictionary(int languageId)
    {
        Debug.Log("GenerateDictionary: " + languageId);
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        FieldInfo[] members = languageTags.GetType().GetFields();

        foreach (FieldInfo fi in members)
        {
            string key, value;
            // set key to fields name
            key = fi.Name;

            // get all the fields values
            string[] values = (string[])fi.GetValue(languageTags);
            if (values != null)
            {
                value = values[languageId];
            }
            else
            {
                Debug.Log(key + ": null");
                value = key;
            }

            dictionary.Add(key, value);

        }

        return dictionary;
    }

    public static string GetFilePath(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName;

        try
        {
            if (!File.Exists(filePath))
            {
                string tempPath = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);

                // Android only use WWW to read file
                WWW reader = new WWW(tempPath);
                while (!reader.isDone) { }

                System.IO.File.WriteAllBytes(filePath, reader.bytes);
            }
            return filePath;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        return null;


    }
}
