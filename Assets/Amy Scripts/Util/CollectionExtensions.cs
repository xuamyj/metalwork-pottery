using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Put this in a static class in your project
public static class CollectionExtensions
{
    // Extension method for List<T>
    public static string ToDebugString<T>(this List<T> list, string separator = ", ")
    {
        if (list == null)
            return "null";

        if (list.Count == 0)
            return "[]";

        return "[" + string.Join(separator, list.Select(item => item?.ToString() ?? "null")) + "]";
    }

    // Extension method for HashSet<T>
    public static string ToDebugString<T>(this HashSet<T> hashSet, string separator = ", ")
    {
        if (hashSet == null)
            return "null";

        if (hashSet.Count == 0)
            return "{}";

        return "{" + string.Join(separator, hashSet.Select(item => item?.ToString() ?? "null")) + "}";
    }

    // Extension method for Dictionary<TKey, TValue>
    public static string ToDebugString<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        if (dict == null)
            return "null";

        if (dict.Count == 0)
            return "{}";

        StringBuilder sb = new StringBuilder();
        sb.Append("{\n");

        foreach (var kvp in dict)
        {
            string valueString;

            // Check if value is a collection itself
            if (kvp.Value is ICollection<object> collection)
                valueString = string.Join(", ", collection.Select(item => item?.ToString() ?? "null"));
            else
                valueString = kvp.Value?.ToString() ?? "null";

            sb.Append($"  {kvp.Key?.ToString() ?? "null"} => {valueString}\n");
        }

        sb.Append("}");
        return sb.ToString();
    }

    // Special case for Dictionary with HashSet or List values
    public static string ToDebugString<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> dict)
    {
        if (dict == null)
            return "null";

        if (dict.Count == 0)
            return "{}";

        StringBuilder sb = new StringBuilder();
        sb.Append("{\n");

        foreach (var kvp in dict)
        {
            string valueString = kvp.Value.ToDebugString();
            sb.Append($"  {kvp.Key?.ToString() ?? "null"} => {valueString}\n");
        }

        sb.Append("}");
        return sb.ToString();
    }

    // Similar method for Dictionary with List values
    public static string ToDebugString<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict)
    {
        if (dict == null)
            return "null";

        if (dict.Count == 0)
            return "{}";

        StringBuilder sb = new StringBuilder();
        sb.Append("{\n");

        foreach (var kvp in dict)
        {
            string valueString = kvp.Value.ToDebugString();
            sb.Append($"  {kvp.Key?.ToString() ?? "null"} => {valueString}\n");
        }

        sb.Append("}");
        return sb.ToString();
    }
}
