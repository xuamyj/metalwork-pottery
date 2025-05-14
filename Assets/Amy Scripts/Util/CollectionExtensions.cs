using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

        var entries = dict.Select(kvp =>
        {
            string valueString;

            // Check if value is a collection itself
            if (kvp.Value is ICollection<object> collection)
                valueString = string.Join(", ", collection.Select(item => item?.ToString() ?? "null"));
            else
                valueString = kvp.Value?.ToString() ?? "null";

            return $"{kvp.Key?.ToString() ?? "null"} => {valueString}";
        });

        return "{" + string.Join(", ", entries) + "}";
    }

    // Special case for Dictionary with HashSet values
    public static string ToDebugString<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> dict)
    {
        if (dict == null)
            return "null";

        if (dict.Count == 0)
            return "{}";

        var entries = dict.Select(kvp =>
            $"{kvp.Key?.ToString() ?? "null"} => {kvp.Value.ToDebugString()}"
        );

        return "{" + string.Join(", ", entries) + "}";
    }

    // Similar method for Dictionary with List values
    public static string ToDebugString<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict)
    {
        if (dict == null)
            return "null";

        if (dict.Count == 0)
            return "{}";

        var entries = dict.Select(kvp =>
            $"{kvp.Key?.ToString() ?? "null"} => {kvp.Value.ToDebugString()}"
        );

        return "{" + string.Join(", ", entries) + "}";
    }
}
