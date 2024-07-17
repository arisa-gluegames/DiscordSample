using GlueGames.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class KibotuPreloadAsset : Singleton<KibotuPreloadAsset>
{
    private Texture2D _loadedTexture;

    private void OnImageLoaded(Texture2D loadedTexture)
    {
        LogManager.Log("Kibotu loaded texture successfully");
    }

    public void PreloadImage(string url)
    {
        LogManager.Log("Kibotu request preloading image from: " + url);
        StartCoroutine(LoadImageFromURL(url, this.OnImageLoaded));
    }

    private IEnumerator LoadImageFromURL(string url, System.Action<Texture2D> onLoaded)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ConnectionError ||
                uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                LogManager.LogWarning("Kibotu an error occurred when loading image from URL: " + uwr.error);
            }
            else
            {
                _loadedTexture = DownloadHandlerTexture.GetContent(uwr);
                onLoaded?.Invoke(_loadedTexture);
            }
        }
    }

    public Texture2D GetPreloadedTexture()
    {
        return _loadedTexture;
    }
}