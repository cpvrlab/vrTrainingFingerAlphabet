using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Converts JsonArrayString to List of Sturcts
/// 
/// Author: https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Converts JsonArrayString to List of structs
    /// </summary>
    /// <typeparam name="T">Struct type</typeparam>
    /// <param name="json">JsonArrayString</param>
    /// <returns>List of given structs</returns>
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    /// <summary>
    /// Converts List of structs to JsonArrayString
    /// </summary>
    /// <typeparam name="T">Struct type</typeparam>
    /// <param name="array">List of structs</param>
    /// <returns>JsonArrayString</returns>
    public static string ToJson<T>(List<T> array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }


    /// <summary>
    /// Converts List of struct to JsonArrayString
    /// </summary>
    /// <typeparam name="T">Struct type</typeparam>
    /// <param name="array">List of structs</param>
    /// <param name="prettyPrint">make the string readable</param>
    /// <returns>JsonArrayString</returns>
    public static string ToJson<T>(List<T> list, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = list;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}
