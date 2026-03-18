using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Utility
{
    public static string stageDataPath = Application.persistentDataPath + "/stageData.json";
    public static string StageDataSavePrefKey = "StageDataSaved";


}
public static class Rand
{
    public static System.Random r = new System.Random();

    public static bool GetBool()
    {
        return r.Next(0, 2) == 1;
    }
    /// <summary>
    /// odds 1-50
    /// </summary>
    public static bool GetBool(int odds = 50)
    {
        odds = Mathf.Clamp(odds, 1, 50);
        return r.Next(0, 100) < odds;
    }

    public static int GetInt(int max = 1)
    {
        return r.Next(0, max);
    }

    public static int GetInt(int min, int max)
    {
        return r.Next(min, max);
    }

    public static int[] GetIntArray(int length, int minVal, int maxVal)
    {
        int[] a = new int[length];
        for (int i = 0; i < length; i++)
        {
            a[i] = r.Next(minVal, maxVal);
        }
        return a;
    }

    public static int[] GetIntUniqueArray(int length, int minVal, int maxVal)
    {
        int range = maxVal - minVal;
        if (length > range)
        {
            Debug.LogError($"length ({length}) must be less than or equal to range ({range})");
            return new int[0];
        }

        // Fisher-Yates shuffle algorithm for better performance
        int[] pool = new int[range];
        for (int i = 0; i < range; i++)
        {
            pool[i] = minVal + i;
        }

        int[] result = new int[length];
        for (int i = 0; i < length; i++)
        {
            int randomIndex = r.Next(i, range);
            result[i] = pool[randomIndex];
            pool[randomIndex] = pool[i];
        }

        return result;
    }

    public static float GetFloat(float max = 1)
    {
        return (float)r.NextDouble() * max;
    }

    public static double GetDouble(double max = 1)
    {
        return (double)r.NextDouble() * max;
    }

    public static double GetDouble(double min, double max)
    {
        return min + (double)r.NextDouble() * (max-min);
    }

    public static float GetFloat(float min, float max)
    {
        return min + GetFloat(max-min);
    }

    public static T GetEnum<T>() where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("Rand.GetEnum :: T must be an enum");
        }

        T[] a = (T[])Enum.GetValues(typeof(T));
        return a[GetInt(a.Length)];
    }

    public static T GetFromArray<T>(T[] a)
    {
        return a[GetInt(a.Length)];
    }
}