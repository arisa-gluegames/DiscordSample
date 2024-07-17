using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class Conversion
{
    public static string BytesToString(long byteCount)
    {
        return BytesToString((decimal)byteCount);
    }

    public static string BytesToString(float byteCount)
    {
        return BytesToString((decimal)byteCount);
    }

    private static string BytesToString(decimal byteCount)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        if (byteCount == 0)
            return "0" + suffixes[0];

        int mag = (int)Math.Log((double)byteCount, 1024);
        decimal adjustedSize = byteCount / (1L << (mag * 10));

        return string.Format("{0:n1} {1}", adjustedSize, suffixes[mag]);
    }
}