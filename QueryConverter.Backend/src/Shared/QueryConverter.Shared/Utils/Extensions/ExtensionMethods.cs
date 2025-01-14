﻿using Newtonsoft.Json;

namespace QueryConverter.Shared.Utils.Extensions;

public static class ExtensionMethods
{
    public static string PrettyJson(this string jsonString)
    {
        var parsedJson = JsonConvert.DeserializeObject(jsonString);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    }

    public static bool IsCollectionType(Type type)
    {
        return type.IsArray;
    }

    public static bool IsValidElementType(Type type)
    {
        return type != null && (
            type == typeof(int) ||
            type == typeof(uint) ||
            type == typeof(string) ||
            type == typeof(bool) ||
            type.IsEnum);
    }

    /// <summary>
    /// For switch same case. Example: var x when x.In(value, value, value) => "value, value, or value",
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool In<T>(this T value, params T[] values) => values.Contains(value);

    // TODO: do smth with to string because return value is number not a query
    public static int SplitQuery(ref string query)
    {
        var resultQuery = query.Split('\n').Length + 2;

        return resultQuery;
    }
}
