namespace GlueGames.Utilities
{
    public static class Commons
    {

#if UNITY_EDITOR
        public static string StartingScene;
        public static string StartingSceneName;
        public static bool LaunchNormally = true;
#endif

        //TODO: Define a better representation of Default Keys for RemoteConfig
        #region RemoteConfig
        // ECONOMY
        public const string ConfigEconLivesInitialBalance = "lives_initial_balance";
        public const string ConfigEconLivesRegenTime = "lives_regen_time";
        public const string ConfigEconLivesMaxValue = "lives_max_value";
        public const string ConfigEconRewardCoinsPerLevel = "reward_coins_per_level";
        public const string ConfigEconRewardCoinsPerLetter = "reward_coins_per_letter";

        // PURCHASING
        public const string ConfigPurchasingGoogleEnabled = "enable_google_purchasing";
        public const string ConfigPurchasingAppleEnabled = "enable_apple_purchasing";
        public const string ConfigPurchasingItemCooldown = "global_item_cooldown";

        // GAMEPLAY
        public const string ConfigKeyboardType = "keyboard_type";
        public const string ConfigWheelRandomCharacterCount = "wheel_keyboard_random_character_count";
        public const string ConfigTutorialLevelCount = "tutorial_level_count";
        public const string ConfigPvpLevelUnlock = "pvp_level_unlock";
        public const string ConfigLevelSetCount = "level_set_count";
        public const string ConfigCoopLevelSetCount = "coop_level_set_count";
        public const string ConfigInitialLevelsVersion = "latestVersionWhenTutorialLevelsChanged";
        public const string ConfigInitialChapterVersion = "latestVersionWhenFirstChapterBackgroundsChanged";
        
        // AB TEST CASES
        public const string ConfigABGameMode = "ab_game_mode";

        // LOGIN
        public const string ConfigIsEnableFacebookLogin = "is_enable_facebook_login";
        #endregion

        #region PlayerPrefs

        #region Sounds
        public static string IsSFXOn => "IsSFXOn";
        public static string IsBGMOn => "IsBGMOn";
        public static string IsBGMLoop => "IsBGMLoop";
        #endregion

        #region Configuration
        public static string IsOfflineConfig => "multiplayer_isOffline";
        #endregion

        #region Authentication
        public static string SwitchDataKey => "authentication.switchData";
        public static string LoginHistoryKey => "authentication.firstLogin";
        public static string AuthenticationTokenKey => "nakama.authToken";
        public static string RefreshTokenKey => "nakama.refreshToken";
        public static string NakamaAuthMethodKey => "nakama.authMethod";
        public static string LastAuthMethodKey => "authentication.authMethod";
        public static string DefaultUserId => "Guest";
        #endregion

        #endregion

        #region Sounds
        public static string SFXBus => "General";
        public static string BGMPlaylist => "BackgroundMusic";
        #endregion

        #region Addressables
        public static string AllAssetLabel => "All";
        public static string RuntimeAssetLabel => "Runtime";
        public static string OnScreenKeyboardAddress => "OnScreenKeyboardCanvas";
        public static string GameLevelManagerAddress => "GameLevelManager";
        public static string UILevelManagerAddress => "UILevelManager";
        #endregion

        #region Format
        public static string HourMinuteFormat = @"%h\:mm";
        public static string MinuteSecondsFormat = @"%m\:ss";
        public static string SecondsFormat = @"%s";
        #endregion
    }

}