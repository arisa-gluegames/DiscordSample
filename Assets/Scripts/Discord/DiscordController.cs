using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using GlueGames.Utilities;

public class DiscordController : SingletonPersistent<DiscordController> 
{
    const long DiscordAppId = 1262324555419090966;
    public Discord.Discord _discord;

    // Use this for initialization
    void Start()
    {
        System.Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", "0");
        _discord = new Discord.Discord(DiscordAppId, (System.UInt64)Discord.CreateFlags.Default);
        var activityManager = _discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            State = "Still Testing",
            Details = "First Testing"
        };
        activityManager.UpdateActivity(activity, (res) =>
        {
            if (res == Discord.Result.Ok)
            {
                Debug.LogError("Everything is fine!");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        _discord.RunCallbacks();
    }
}