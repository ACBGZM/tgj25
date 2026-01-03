using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class YieldHelper
{
    public static IEnumerator WaitForSeconds(float totalWaitTime, bool useRealTime = false)
    {
        float time = 0.0f;
        while (time < totalWaitTime)
        {
            time += (useRealTime ? Time.unscaledDeltaTime : Time.deltaTime);
            yield return null;
        }
    }

    public static IEnumerator WaitUntil(Func<bool> func)
    {
        while (!func())
        {
            yield return null;
        }
    }
}

public static class RandomHelper
{
    public static T PickOne<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            return default;
        }

        return list[Random.Range(0, list.Count)];
    }

    public static List<T> PickRandomList<T>(List<T> source, int count)
    {
        List<T> copy = new List<T>(source);
        List<T> result = new List<T>();

        count = Mathf.Min(count, copy.Count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, copy.Count);
            result.Add(copy[index]);
            copy.RemoveAt(index);
        }

        return result;
    }
}