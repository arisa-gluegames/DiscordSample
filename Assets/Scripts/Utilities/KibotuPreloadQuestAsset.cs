using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GlueGames.Utilities;
using JetBrains.Annotations;

public class KibotuPreloadQuestAsset : SingletonPersistent<KibotuPreloadQuestAsset>
{
    public int maxRetries = 1; // Maximum number of retries if the download fails
    public float retryDelay = 1.0f; // Delay between retries in seconds
    [CanBeNull] public bool? isPreloadingActiveQuest = null;
    [CanBeNull] public bool? isPreloadingFinished = false;

    // Method to set the image URL and start loading the image
    public void LoadImage(Image targetImage, string url , Action<bool> onComplete)
    {
        try
        {
            if (KibotuSpriteCache.Instance.TryGetSprite(url, out Sprite cachedSprite))
            {
                Debug.Log("Sprite loaded from cache; For url: " + url);
                targetImage.sprite = cachedSprite;
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.Log("Sprite not in cache - downloading... For url: " + url);
                // targetImage.gameObject.SetActive(false);
                StartCoroutine(DownloadImageWithRetry(targetImage, url, maxRetries, (isLoaded =>
                {
                    onComplete?.Invoke(isLoaded);
                })));
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Sprite loading error for URL: " + url + "; " + ex.Message);
            onComplete?.Invoke(false);
        }
    }

    public IEnumerator PreloadImage(string url)
    {
        return DownloadImageWithRetry(null, url, maxRetries, null);
    }

    private IEnumerator LoadImageCoroutine(Image targetImage, string url)
    {
        var completed = false;
        KibotuPreloadQuestAsset.Instance.LoadImage(targetImage, url, (isLoaded) =>
        {
            completed = true;
        });
        
        // Wait for Kibotu or time out after 10 seconds.
        float maxWait = 10.0f;
        float timer = 0.0f;
        while (!completed && timer < maxWait)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitUntil(() => completed);
        }
    }
    
    public Task RunMultipleCoroutinesAndWait(params IEnumerator[] coroutines)
    {
        return Task.WhenAll(coroutines.Select(c => StartCoroutineAndWait(c)).ToArray());
    }

    private async Task StartCoroutineAndWait(IEnumerator coroutine)
    {
        var tcs = new TaskCompletionSource<bool>();

        StartCoroutine(RunCoroutine(coroutine, tcs));

        await tcs.Task;
    }

    private IEnumerator RunCoroutine(IEnumerator coroutine, TaskCompletionSource<bool> tcs)
    {
        yield return coroutine;
        tcs.SetResult(true);
    }

    public IEnumerator PreloadActiveQuest(List<string> imageUrls)
    {
        Instance.isPreloadingActiveQuest = true;
        List<Coroutine> coroutines = new List<Coroutine>();
        foreach (var imageUrl in imageUrls)
        {
            coroutines.Add(StartCoroutine(LoadImageCoroutine(null, imageUrl)));
        }

        // Wait for all coroutines to complete
        foreach (var coroutine in coroutines)
        {
            yield return coroutine;
        }

        Instance.isPreloadingActiveQuest = false;
        Instance.isPreloadingFinished = true;
    }
    
    IEnumerator DownloadImageWithRetry(Image targetImage, string url, int retries, Action<bool> onComplete)
    {
        int attempt = 0;
        bool success = false;

        while (attempt < retries && !success)
        {
            Debug.Log($"Attempt {attempt + 1} to download image from URL: {url}");
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to download image from url {url}: {uwr.error}");
                    attempt++;
                    if (attempt < retries)
                    {
                        Debug.Log($"Retrying in {retryDelay} seconds; url: {url}");
                        yield return new WaitForSeconds(retryDelay);
                    }
                }
                else
                {
                    Debug.Log("Image downloaded successfully");
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    if (targetImage)
                    {
                        targetImage.sprite = sprite;
                    }
                    else
                    {
                        Debug.Log("Image from URL: " + url + " downloaded successfully and cached");
                    }

                    onComplete?.Invoke(true);

                    KibotuSpriteCache.Instance.AddSprite(url, sprite);
                    success = true;
                }
            }
        }

        if (!success)
        {
            Debug.LogError("Failed to download image after multiple attempts");
            onComplete?.Invoke(false);
        }
    }
}