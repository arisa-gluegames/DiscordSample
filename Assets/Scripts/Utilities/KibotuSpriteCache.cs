using System;
using System.Collections;
using System.Collections.Generic;
using GlueGames.Utilities;
using UnityEngine;
using UnityEngine.Networking;

public class KibotuSpriteCache : SingletonPersistent<KibotuSpriteCache>
{
    private Dictionary<string, Sprite> spriteCache;

    private KibotuSpriteCache()
    {
        spriteCache = new Dictionary<string, Sprite>();
    }

    public bool TryGetSprite(string url, out Sprite sprite)
    {
        // Debug.Log("DBG: " + spriteCache.Count + " sprites in cache. getting for url: " + url);
        return spriteCache.TryGetValue(url, out sprite);
    }

    public void AddSprite(string url, Sprite sprite)
    {
        // Debug.Log("DBG: " + spriteCache.Count + " sprites in cache. adding for url: " + url);
        spriteCache[url] = sprite;
    }

    public void ClearCache()
    {
        spriteCache.Clear();
    }
}