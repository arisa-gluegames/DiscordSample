#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using GlueGames.Utilities;

public static class DeleteSaves 
{
    private static readonly string SavePath = Application.persistentDataPath;
    private static readonly string FileExtension = ".json";
    private static readonly string WalletFileName = "currencydata_";
    private static readonly string PlayerDataFileName = "playerdata_";

    [MenuItem("Tools/GlueGames/Delete Saves/Delete PlayerPrefs")]
    static public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/GlueGames/Delete Saves/Delete Authentication")]
    static public void DeleteAuthenticationData()
    {
        PlayerPrefs.DeleteKey(Commons.AuthenticationTokenKey);
        PlayerPrefs.DeleteKey(Commons.NakamaAuthMethodKey);
        PlayerPrefs.DeleteKey(Commons.LastAuthMethodKey);
    }

    [MenuItem("Tools/GlueGames/Delete Saves/Delete Wallet")]
    static public void DeleteWallet()
    {
        string fileName = WalletFileName + Commons.DefaultUserId;
        string filePath = Path.Combine(SavePath, fileName + FileExtension);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Successfully deleted Wallet data");
        } 
    }

    [MenuItem("Tools/GlueGames/Delete Saves/Delete PlayerData")]
    static public void DeletePlayerData()
    {
        string fileName = PlayerDataFileName + Commons.DefaultUserId;
        string filePath = Path.Combine(SavePath, fileName + FileExtension);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Successfully deleted Player data");
        }
    }

    [MenuItem("Tools/GlueGames/Delete Saves/Delete Addressable Cache")]
    static public void DeleteAddressableCache()
    {
        Caching.ClearCache();
    }

    [MenuItem("Tools/GlueGames/Delete Saves/Delete All", false, 1)]
    static public void DeleteAll()
    {
        DeleteWallet();
        DeletePlayerData();
        DeletePlayerPrefs();
        DeleteAddressableCache();
    }
}
#endif